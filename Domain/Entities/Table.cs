using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Table
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Capacity { get; set; }
        public bool Status { get; private set; }

        // Contructor con validación
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
        public void Occupy()
        {

        }
        public void Release()
        {

        }
    }
}
