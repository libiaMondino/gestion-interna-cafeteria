
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Category
    {
        // Atributos
        public int CategoryId { get; set; }

        public string CategoryName { get; private set; } = string.Empty;

        public string? CategoryDescription { get; set; }

        // Propiedad de navegación
        public ICollection<Product> Products { get; set; }
            = new List<Product>();

        // Constructor
        public Category(string name)
        {
            ChangeName(name);
        }

        // Métodos
        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "El nombre de la categoría no puede estar vacío.");
            }

            CategoryName = name.Trim();
        }
    }
}
