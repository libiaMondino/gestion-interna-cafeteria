using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPasswordHashingService
    {
        string HashPassword(User user, string password);

        bool VerifyPassword(User user, string password);
    }
}
