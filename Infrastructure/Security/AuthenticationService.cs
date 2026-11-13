using Application.Interfaces;
using Application.Models.Users;
using Domain.Entities;
using Domain.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Security
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IGenericRepository<User> _repository;
        private readonly IConfiguration _configuration;
        public AuthenticationService(IGenericRepository<User> repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<string> Authenticate( CredentialsLoginDto credentials)
        {
            // Busca si el usuario existe
            var user = await _repository.GetByFilterAsync(u => credentials.UserEmail == u.UserEmail);
            var isUserNotFound= user is null;
            var isUserInactive = user?.IsActive == false;
            var isPasswordInvalid= user?.UserPasswordHash != credentials.UserPasswordHash;

            if ( isUserNotFound || isUserInactive || isPasswordInvalid)
                throw new InvalidCredentialException();

            // Clave secreta y algoritmo de firma
            var securityKey = new SymmetricSecurityKey(
                              Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]!));
            var signature = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Payload y sus Claims
            var claimsForToken = new List<Claim>
            {
                new("sub", user.UserId.ToString()),
                new("email", user.UserEmail),
                new("role", user.UserRole.ToString()) // Al ser un Enum tiene que convertirse a String
            };

            // Token 
            var jwtToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(8), // Duración token: una jornada laboral
                signature);

            // Serialización : header.payload.signature
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
