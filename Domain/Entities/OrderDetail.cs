using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public int KitchenOrderId { get; set; }
        public int ProductId { get; set; }
        
        //Método
        public decimal CalculateSubtotal() 
        {
            return UnitPrice * Quantity;
        }
    }
}
