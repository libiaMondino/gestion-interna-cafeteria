using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;
namespace Domain.Entities
{
    public class Table
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Capacity { get; set; }
        public string Status { get; private set; } = "Libre";


        // Contructor con validación
        public Table(int number, int capacity)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException("El número de la mesa debe ser mayor a cero");
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException("La capacidad máxima de personas de la mesa debe ser mayor a cero");
            
            Number = number;
            Capacity = capacity;
            Status = "Libre";     
        }
        public void Occupy()
        {
            if (Status == "Ocupada")
                throw new ArgumentException("La mesa ya está ocupada");
            Status = "Ocupada";
        }
        public void Release()
        {
            if (Status == "Libre")
                throw new ArgumentException("La mesa ya estaba liberada");
            Status = "Libre";
        }

        
    }
}
