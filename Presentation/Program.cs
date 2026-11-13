using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Domain.Interfaces;
using Application.Interfaces;
using Application.Services;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Presentation.Middlewares;


var builder = WebApplication.CreateBuilder(args);

// Agregado del ApplicationDbContext 
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<ApplicationDBContext>(options =>
options.UseSqlServer(connectionString));

builder.Services.AddControllers();

builder.Services.AddOpenApi();

//Inyección de dependencias
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof( GenericRepository<>));
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITableService, TableService>();


// Verificación de tokens que llegan en cada una de las request
builder.Services.AddAuthentication("Bearer")
.AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    // Se crea un objeto de reglas para la autenticación token
    options.TokenValidationParameters = new()
    {
        // Validaciones
        ValidateIssuer = true, // quién lo emitió
        ValidateAudience = true, // para quién fue emitido
        ValidateIssuerSigningKey = true, // ¿ La firma es auténtica?

        // Valores esperados
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],

        // // Carga la clave secreta para verificar la firma del token.
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]!)),

        // Le indica a [Authorize(Roles = "...")] qué claim leer del token
        RoleClaimType = "role",
        NameClaimType = "sub",

        // Elimina los 5 minutos de tolerancia por defecto cuando expira el token
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
