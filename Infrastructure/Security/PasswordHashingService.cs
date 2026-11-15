using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Security
{
    public class PasswordHashingService : IPasswordHashingService
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        public PasswordHashingService(IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(User user, string password)
        {
            return _passwordHasher.VerifyHashedPassword(user, user.UserPasswordHash, password)
                != PasswordVerificationResult.Failed;
        }
    }
}
