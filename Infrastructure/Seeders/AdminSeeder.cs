using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Application.Interfaces;
namespace Infrastructure.Seeders
{
    public class AdminSeeder
    {
        private readonly IGenericRepository<User> _repository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHashingService _hashingService;

        public AdminSeeder( 
            IGenericRepository<User> repository, 
            IConfiguration configuration,
            IPasswordHashingService hashingService)
        {
            _repository = repository;
            _configuration = configuration;
            _hashingService = hashingService;
        }

        public async Task SeedAsync()
        {
            // Verificar si ya existe uno o varios usuarios con rol admin
            var admin = await _repository.GetByFilterAsync(user => user.UserRole == UserRole.Admin);

            
            if (admin is null)
            {
                // Se leen las crendenciales desde appsettings
                var adminEmail = _configuration["AdminUser:Email"];
                var adminPassword = _configuration["AdminUser:Password"];
                if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
                    return;

                // Se crea admin
                var adminCreated = new User
               (
                    name: "Admin",
                    lastName: "-",
                    email: adminEmail,
                    role: UserRole.Admin
               );
                // Se hace el hash de la contraseña y se setea
                // Se hace de esta forma porque se configuró el atributo UserPasswordHash como private set
                adminPassword = _hashingService.HashPassword( adminCreated, adminPassword );
                adminCreated.SetPasswordHash(adminPassword);

                _repository.Add(adminCreated);
            }
                
        }
    }
}
