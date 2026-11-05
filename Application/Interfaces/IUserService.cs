using Application.Models.Users;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();

        Task<UserDto?> GetByIdAsync(int id);

        Task<UserDto> CreateAsync(CreateUserDto userDto);

        Task<bool> UpdateAsync(int id, UpdateUserDto userDto);

        Task<bool> ActivateAsync(int id);

        Task<bool> DeactivateAsync(int id);
    }
}