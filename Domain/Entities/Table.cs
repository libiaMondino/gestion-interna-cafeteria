
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Table
    {
        // Propiedades
        public int Id { get; set; }

        public int Number { get; private set; }

        public int Capacity { get; private set; }

        public bool Status { get; private set; }

        // Propiedad de navegación
        public ICollection<Order> Orders { get; set; }
            = new List<Order>();

        // Constructor con validación
        public Table(int number, int capacity)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(number),
                    "El número de mesa debe ser mayor que cero.");

            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(capacity),
                    "La capacidad de la mesa debe ser mayor que cero.");

            Number = number;
            Capacity = capacity;
            Status = true;
        }

        // Métodos
        public void Occupy()
        {
            if (!Status)
                throw new InvalidOperationException(
                    "La mesa ya está ocupada.");

            Status = false;
        }

        public void Release()
        {
            if (Status)
                throw new InvalidOperationException(
                    "La mesa ya está libre.");

            Status = true;
        }
    }
}
