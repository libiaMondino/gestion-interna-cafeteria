using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        private string PasswordHash { get; set; } = string.Empty;
        public UserRole UserRole { get; private set; } 
        public bool IsActive { get; private set; }

        //Métodos
        public void ChangeRole (string role)
        {

        }

        public void ActivateUser ()
        {
            
        }

        public void DeactivateUser()
        {

        }
    }
}
