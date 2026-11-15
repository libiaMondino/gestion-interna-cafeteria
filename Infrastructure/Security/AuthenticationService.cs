using Application.Exceptions;
using Application.Interfaces;
using Application.Models.Users;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Security
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly SymmetricSecurityKey _signingKey;
        private readonly string _issuer;
        private readonly string _audience;

        public AuthenticationService(
            IGenericRepository<User> userRepository,
            IPasswordHashingService passwordHashingService,
            SymmetricSecurityKey signingKey,
            string issuer,
            string audience)
        {
            _userRepository = userRepository;
            _passwordHashingService = passwordHashingService;
            _signingKey = signingKey;
            _issuer = issuer;
            _audience = audience;
        }

        public async Task<string> Authenticate(CredentialsLoginDto credentials)
        {
            var validationResults = new List<ValidationResult>();

            var emailIsValid = Validator.TryValidateValue(credentials.UserEmail,
                new ValidationContext(credentials), validationResults,
                new ValidationAttribute[] { new RequiredAttribute(), new EmailAddressAttribute(), new StringLengthAttribute(254) });
            var passwordIsValid = Validator.TryValidateValue(credentials.Password,
                new ValidationContext(credentials), validationResults,
                new ValidationAttribute[] { new RequiredAttribute() });

            if (!emailIsValid || !passwordIsValid)
                throw new ArgumentException(string.Join(" ", validationResults.Select(result => result.ErrorMessage)));

            var user = await _userRepository.GetByFilterAsync(
                user => user.UserEmail == credentials.UserEmail.Trim()
            );

            if (user == null || !user.IsActive || !_passwordHashingService.VerifyPassword(user, credentials.Password))
                throw new InvalidCredentialsException();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };

            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(60),
                signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
