
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;
namespace Domain.Entities
{
    public class Table
    {
        // Propiedades
        public int Id { get; set; }
        public int Number { get; set; }
        public int Capacity { get; set; }
        public bool Status { get; private set; }

        // Propiedad de navegación
        public ICollection<Order> Orders { get; set; }
            = new List<Order>();

        // Constructor con validación
        public Table(int number, int capacity)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException("Table number must be larger than zero");
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException("Table capacity must be larger than zero");
            
            Number = number;
            Capacity = capacity;
            Status = true;     
        }

        // Métodos
        public void Occupy()
        {

        }

        public void Release()
        {

        }
    }
}
