using Domain.Enums;

namespace Application.Models.Users
{
    public class UpdateUserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string UserLastName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
    }
}