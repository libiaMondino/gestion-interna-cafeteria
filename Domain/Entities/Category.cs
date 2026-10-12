using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Category
    {   // atributos
        public int Id { get; set; } // Guid
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        //constructor
        public Category(string name)
        {
            ChangeName(name);//valida que el nombre este
        }
        //método
        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) // para comprobar si el valor es nulo, cadena vacía o que contiene espacios
            {
                throw new ArgumentException("El nombre de la categoría no puede estar vacío.");
            }   //throw.. indica que el valor recibido no es válido, interrumpe la ejecución del metodo, no se cambia el nombre
            Name = name;

        }

    }
}
