using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions
{
    // Hereda de la clase padre Exception( ahora se podrá lanzar con throw y atrapar con catch)
    public class InvalidCredentialsException : Exception 
    {
        public InvalidCredentialsException(): base("Email o contraseña incorrectos") //Llama al constructor de Exception
        {
        }
    }
}
