using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public bool Status  { get; set; } // = true (que es lo mismo que "Activo")
        public decimal Total { get; set; }
        public int TableId { get; set; }
        public int UserId { get; set; }

        //Métodos
        public void AddKitchenOrder(KitchenOrder kitchenOrder) { }
        public void CalculateTotal() { }
        public void Close() { }
    }
}

