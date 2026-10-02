using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Domain.Enums;
namespace Domain.Entities
{
    public class StockMovement
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Quantity { get; set; }
        public StockMovementType MovementType { get; set; }
        public string Reason { get; set; } = string.Empty;
        
        // Foreign keys
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int IngredientId { get; set; }
        public int OrderDetailId { get; set; }

        //Métodos
        public bool Validate()
        {
            return true; //No es así el método, está así para que no lance error
        }
    }
}
