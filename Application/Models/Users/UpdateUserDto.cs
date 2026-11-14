using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Models.Users
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string UserLastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        public string UserEmail { get; set; } = string.Empty;

        [EnumDataType(typeof(UserRole),
            ErrorMessage = "El rol ingresado no es válido.")]
        public UserRole UserRole { get; set; }
    }
}