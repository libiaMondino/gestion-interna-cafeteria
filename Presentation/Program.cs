using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
//using Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Agregado del ApplicationDbContext 
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<ApplicationDBContext>(options =>
options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Inyección de dependencias
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof( GenericRepository<>));

//builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
