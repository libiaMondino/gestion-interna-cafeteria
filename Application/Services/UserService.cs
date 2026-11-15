using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Application.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace Application.Services
{
        public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;

        public UserService(IGenericRepository<User> userRepository, IPasswordHashingService passwordHashingService)
        {
            _userRepository = userRepository;
            _passwordHashingService = passwordHashingService;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(user => new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserLastName = user.UserLastName,
                UserEmail = user.UserEmail,
                UserRole = user.UserRole,
                IsActive = user.IsActive
            }).ToList();
        }
        public async Task<UserDto?> GetByIdAsync(int id) //busca usuario por id
        {
            if (id <= 0)
                throw new ArgumentException("El id del usuario debe ser mayor que cero.");

            var user = await _userRepository.GetIdAsync(id);

            if (user == null)
                return null;

            return new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserLastName = user.UserLastName,
                UserEmail = user.UserEmail,
                UserRole = user.UserRole,
                IsActive = user.IsActive
            };
        }
        public async Task<UserDto> CreateAsync(CreateUserDto userDto)//recibe los datos del usuario mediante el createuserdto y devuelve un user dto
        {
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(userDto, new ValidationContext(userDto), validationResults, true))
                throw new ArgumentException(string.Join(" ", validationResults.Select(result => result.ErrorMessage)));

            var existingUser = await _userRepository.GetByFilterAsync(
                user => user.UserEmail == userDto.UserEmail.Trim()
            );

            if (existingUser != null)
                throw new ArgumentException("Ya existe un usuario con ese email.");

            var user = new User(userDto.UserName, userDto.UserLastName, userDto.UserEmail, userDto.UserRole);

            user.SetPasswordHash(_passwordHashingService.HashPassword(user, userDto.Password));

            _userRepository.Add(user);

            return new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserLastName = user.UserLastName,
                UserEmail = user.UserEmail,
                UserRole = user.UserRole,
                IsActive = user.IsActive
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserDto userDto)// <actualizar los daotos de un usuario existente
        {
            if (id <= 0)
                throw new ArgumentException("El id del usuario debe ser mayor que cero.");

            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(userDto, new ValidationContext(userDto), validationResults, true))
                throw new ArgumentException(string.Join(" ", validationResults.Select(result => result.ErrorMessage)));

            var user = await _userRepository.GetIdAsync(id);

            if (user == null)
                return false;

            var existingUser = await _userRepository.GetByFilterAsync(
                user => user.UserEmail == userDto.UserEmail.Trim()
                    && user.UserId != id
            );

            if (existingUser != null)
                throw new ArgumentException("Ya existe un usuario con ese email.");

            user.UserName = userDto.UserName.Trim();
            user.UserLastName = userDto.UserLastName.Trim();
            user.UserEmail = userDto.UserEmail.Trim();
            user.ChangeRole(userDto.UserRole);

            _userRepository.Update(user);

            return true;
        }
        public async Task<bool> ActivateAsync(int id)//activar un usuario mediante el id
        {
            if (id <= 0)
                throw new ArgumentException("El id del usuario debe ser mayor que cero.");

            var user = await _userRepository.GetIdAsync(id);

            if (user == null)
                return false;

            user.ActivateUser();

            _userRepository.Update(user);

            return true;
        }
        public async Task<bool> DeactivateAsync(int id)//desactivar un usuario mediante su id
        {
            if (id <= 0)
                throw new ArgumentException("El id del usuario debe ser mayor que cero.");

            var user = await _userRepository.GetIdAsync(id);

            if (user == null)
                return false;

            user.DeactivateUser();

            _userRepository.Update(user);

            return true;
        }

    }
}