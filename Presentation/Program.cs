using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Infrastructure.Transactions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Presentation.Middlewares;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Agregado del ApplicationDbContext
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<ApplicationDBContext>(options =>
options.UseSqlServer(connectionString));

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

//Inyección de dependencias-----------------------
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof( GenericRepository<>));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();

builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IRecipeDetailService, RecipeDetailService>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
builder.Services.AddScoped<IStockMovementService, StockMovementService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Configuración clave secreta
// Las excepciones no son atrapadas por el global exception middleware(no creado todavía), sino que detiene la ejecución
// Es preferible que la applicación no se ejecute con seguridad incompleta
var secretForKey = builder.Configuration["Authentication:SecretForKey"];

if (string.IsNullOrWhiteSpace(secretForKey))
    throw new InvalidOperationException(
        "La clave secreta de autenticación no está configurada.");

var secretBytes = Encoding.UTF8.GetBytes(secretForKey);

if (secretBytes.Length < 32)
    throw new InvalidOperationException(
        "La clave secreta debe contener al menos 32 bytes.");

var issuer = builder.Configuration["Authentication:Issuer"];
var audience = builder.Configuration["Authentication:Audience"];

if (string.IsNullOrWhiteSpace(issuer) ||
    string.IsNullOrWhiteSpace(audience))
{
    throw new InvalidOperationException(
        "El emisor y la audiencia JWT son obligatorios.");
}

// Creación de la clave de firma
var signingKey = new SymmetricSecurityKey(secretBytes);

// Registración de AuthenticationService
builder.Services.AddScoped<IAuthenticationService>(provider =>
    new AuthenticationService(
        provider.GetRequiredService<IGenericRepository<User>>(),
        provider.GetRequiredService<IPasswordHashingService>(),
        signingKey,
        issuer,
        audience
    ));

// Verificación de tokens que llegan en cada una de las request

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validaciones
            ValidateIssuer = true, // quién lo emitió
            ValidateAudience = true, // para quién fue emitido
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true, // ¿ La firma es auténtica?

            // Valores esperados
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = signingKey,

            // Algoritmo permitido
            ValidAlgorithms = new[]
            {
                SecurityAlgorithms.HmacSha256
            },

            // Le indica a [Authorize(Roles = "...")] qué claim leer del token
            RoleClaimType = "role",
            NameClaimType = "sub",

            // Sin tolerancia adicional de expiración
            ClockSkew = TimeSpan.Zero
        };
    });




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
