using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models.Users
{
    // Son credenciales que llegan en el body del endpoint del Login  
    public record CredentialsLoginDto(
        // record: es una clase de tipo referencia(apunta a un bloque de memoria).
        // Su objetivo es transportar de manera segura e INMUTABLE(solo lectura) por toda la aplicación
        string UserEmail, 
        string UserPasswordHash)
    {
    }
}
