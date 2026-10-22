using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserLastName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPasswordHash { get; private set; } = string.Empty; // el private set impide que otra clse modifique
        public UserRole UserRole { get; private set; } 
        public bool IsActive { get; private set; }

     private User()
        {
        }

        // constructor
        public User(string name, string lastName, string email, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("El apellido es obligatorio.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email es obligatorio.");

            UserName = name;
            UserLastName = lastName;
            UserEmail = email;
            ChangeRole(role);
            IsActive = true;
        }




        //Métodos
        public void ChangeRole(UserRole role)// recibimos un valor del enum
        {
            if (!Enum.IsDefined(typeof(UserRole), role)) // comprobamos que sea uno de los 4 definidos
            {
                throw new ArgumentException("El rol ingresado no es válido.");
            }

            UserRole = role; // si es valido, actualizamos el rol
        }

        public void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentException("El hash de la contraseña es obligatorio.");
            }

            UserPasswordHash = passwordHash;
        }

        public void ActivateUser ()
        {
             IsActive = true;
        }

        public void DeactivateUser()
        {
            IsActive = false;
        }
    }
}
