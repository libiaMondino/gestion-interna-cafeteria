using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces;
namespace Infrastructure.Security
{

    public class PasswordHasherService : IPasswordHasherService
    {
        // A partir de una contraseña en texto plano se genera un hash seguro 
        public string HashUserPassword(string password)
        {

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }

        // Verifica si una contraseña ingresada coincide con su hash almacenado.
        public bool VerifyUserPassword(string password, string hashedPassword)
        {
            var isEqual = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            return isEqual;
        }
    }
} 


