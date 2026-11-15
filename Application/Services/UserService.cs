using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Application.Models.Users;

namespace Application.Services
{
        public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;

        public UserService(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<List<UserDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Task<UserDto?> GetByIdAsync(int id) //busca usuario por id
        {
            throw new NotImplementedException();
        }
        public Task<UserDto> CreateAsync(CreateUserDto userDto)//recibe los datos del usuario mediante el createuserdto y devuelve un user dto
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, UpdateUserDto userDto)// <actualizar los daotos de un usuario existente
        {
            throw new NotImplementedException();
        }
        public Task<bool> ActivateAsync(int id)//activar un usuario mediante el id
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeactivateAsync(int id)//desactivar un usuario mediante su id
        {
            throw new NotImplementedException();
        }

    }
}