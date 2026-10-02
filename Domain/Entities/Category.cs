using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Category
    {
        public int Id { get; set; } // Guid
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        public void ChangeName(string name)
        {

        }

    }
}
