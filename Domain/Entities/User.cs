
using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
    public class User
    {
        // Propiedades
        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string UserLastName { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public string UserPasswordHash { get; private set; } = string.Empty;

        public UserRole UserRole { get; private set; }

        public bool IsActive { get; private set; }

        // Propiedad de navegación
        public ICollection<Order> Orders { get; set; }
            = new List<Order>();

        // Constructor privado para Entity Framework
        private User()
        {
        }

        // Constructor principal
        public User(string name, string lastName, string email, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("El apellido es obligatorio.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email es obligatorio.");

            UserName = name.Trim();
            UserLastName = lastName.Trim();
            UserEmail = email.Trim();

            ChangeRole(role);

            IsActive = true;
        }

        // Métodos

        public void ChangeRole(UserRole role)
        {
            if (!Enum.IsDefined(typeof(UserRole), role))
                throw new ArgumentException(
                    "El rol ingresado no es válido.");

            UserRole = role;
        }

        public void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException(
                    "El hash de la contraseña es obligatorio.");

            UserPasswordHash = passwordHash;
        }

        public void ActivateUser()
        {
            IsActive = true;
        }

        public void DeactivateUser()
        {
            IsActive = false;
        }
    }
}
