using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Infrastructure.Seeders;
using Infrastructure.Transactions;
using Infrastructure.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using Presentation.Middlewares;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// BASE DE DATOS
// --------------------------------------------------

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión DefaultConnection.");
}

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddHttpClient<IExchangeRateService, ExchangeRateService>(client =>
{
    client.BaseAddress = new Uri("https://dolarapi.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddAuthorization();

// --------------------------------------------------
// SWAGGER
// --------------------------------------------------

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Cafetería",
        Version = "v1",
        Description = "API para la gestión interna de la cafetería."
    });

    // Configuración de autenticación JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresá tu token JWT."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// --------------------------------------------------
// INYECCIÓN DE DEPENDENCIAS
// --------------------------------------------------

builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();

builder.Services.AddScoped<AdminSeeder>();

builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IRecipeDetailService, RecipeDetailService>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
builder.Services.AddScoped<IStockMovementService, StockMovementService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IKitchenOrderService, KitchenOrderService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();

// --------------------------------------------------
// CONFIGURACIÓN JWT
// --------------------------------------------------

var secretForKey =
    builder.Configuration["Authentication:SecretForKey"];

if (string.IsNullOrWhiteSpace(secretForKey))
{
    throw new InvalidOperationException(
        "La clave secreta de autenticación no está configurada.");
}

var secretBytes = Encoding.UTF8.GetBytes(secretForKey);

if (secretBytes.Length < 32)
{
    throw new InvalidOperationException(
        "La clave secreta debe contener al menos 32 bytes.");
}

var issuer = builder.Configuration["Authentication:Issuer"];
var audience = builder.Configuration["Authentication:Audience"];

if (string.IsNullOrWhiteSpace(issuer) ||
    string.IsNullOrWhiteSpace(audience))
{
    throw new InvalidOperationException(
        "El emisor y la audiencia JWT son obligatorios.");
}

// Clave de firma
var signingKey = new SymmetricSecurityKey(secretBytes);

// AuthenticationService
builder.Services.AddScoped<IAuthenticationService>(provider =>
    new AuthenticationService(
        provider.GetRequiredService<IGenericRepository<User>>(),
        provider.GetRequiredService<IPasswordHashingService>(),
        signingKey,
        issuer,
        audience
    ));

// --------------------------------------------------
// AUTENTICACIÓN Y VALIDACIÓN DE TOKENS
// --------------------------------------------------

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = signingKey,

            ValidAlgorithms = new[]
            {
                SecurityAlgorithms.HmacSha256
            },

            RoleClaimType = "role",
            NameClaimType = "sub",

            ClockSkew = TimeSpan.Zero
        };

        // Verificación adicional del usuario en cada solicitud
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userIdClaim = context.Principal?
                    .FindFirst("sub")?.Value;

                if (!int.TryParse(userIdClaim, out int userId) ||
                    userId <= 0)
                {
                    context.Fail("Token inválido.");
                    return;
                }

                var userRepository = context.HttpContext
                    .RequestServices
                    .GetRequiredService<IGenericRepository<User>>();

                var user = await userRepository.GetIdAsync(userId);

                // Comprobar que el usuario exista y esté activo
                if (user == null || !user.IsActive)
                {
                    context.Fail("El usuario no existe o está inactivo.");
                    return;
                }

                // Comprobar que el rol del token siga vigente
                var tokenRole = context.Principal?
                    .FindFirst("role")?.Value;

                var currentRole = user.UserRole.ToString();

                if (string.IsNullOrWhiteSpace(tokenRole) ||
                    tokenRole != currentRole)
                {
                    context.Fail("El rol del usuario ha cambiado.");
                }
            }
        };
    });

// --------------------------------------------------
// CONSTRUCCIÓN DE LA APLICACIÓN
// --------------------------------------------------

var app = builder.Build();

// Swagger UI (solo en entorno de desarrollo)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "API Cafetería v1");

        options.DocumentTitle = "API Cafetería";
    });
}

// --------------------------------------------------
// SEEDER
// --------------------------------------------------

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider
        .GetRequiredService<AdminSeeder>();

    await seeder.SeedAsync();
}

// --------------------------------------------------
// PIPELINE HTTP
// --------------------------------------------------

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
