using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
using Application.Services;
//using Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;
using Presentation.Middlewares;
using System.Text;
using Domain.Entities;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Agregado del ApplicationDbContext 
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<ApplicationDBContext>(options =>
options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Inyección de dependencias-----------------------
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof( GenericRepository<>));

//builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
// Registra CategoryService para que los controladores puedan utilizarlo
// a través de ICategoryService mediante inyección de dependencias.

// Registra UserService para que los controladores puedan utilizarlo
// mediante la interfaz IUserService.
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IProductService, ProductService>();

/*Verificación de tokens que llegan en cada una de las request
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateAntiForgeryTokenAttribute IssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKeyResolver = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Authentication: SecretForKey"]!))
        };
    });*/

var secretForKey = Environment.GetEnvironmentVariable("Authentication__SecretForKey");

if (string.IsNullOrWhiteSpace(secretForKey) || Encoding.UTF8.GetByteCount(secretForKey) < 32)
    throw new InvalidOperationException("La variable de entorno Authentication__SecretForKey debe contener una clave de al menos 32 bytes.");

var issuer = builder.Configuration["Authentication:Issuer"];
var audience = builder.Configuration["Authentication:Audience"];

if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
    throw new InvalidOperationException("El emisor y la audiencia JWT son obligatorios.");

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretForKey));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddScoped<IAuthenticationService>(provider => new AuthenticationService(
    provider.GetRequiredService<IGenericRepository<User>>(),
    provider.GetRequiredService<IPasswordHashingService>(),
    signingKey,
    issuer,
    audience));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = signingKey,
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

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
