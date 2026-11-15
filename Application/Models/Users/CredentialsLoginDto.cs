using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Application.Models.Users
{
    // Son credenciales que llegan en el body del endpoint del Login  
    public record CredentialsLoginDto(
        // record: es una clase de tipo referencia(apunta a un bloque de memoria).
        // Su objetivo es transportar de manera segura e INMUTABLE(solo lectura) por toda la aplicación
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [StringLength(254)]
        string UserEmail, 
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        string Password);
}
