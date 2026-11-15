using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Models.Users
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
        public string UserLastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [StringLength(254, ErrorMessage = "El email no puede superar los 254 caracteres.")]
        public string UserEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [EnumDataType(typeof(UserRole),
            ErrorMessage = "El rol ingresado no es válido.")]
        public UserRole UserRole { get; set; }
    }
}
