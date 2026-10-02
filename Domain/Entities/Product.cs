using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ProductType Type { get; set; }
        public bool IsAvailable { get; set; } // En el modelo era tipo BIT
        public int? QuantityStock { get; set; }
        public int? MinimumStock { get; set; }
        public int CategoryId { get; set; }

        //Métodos
        public void ChangeAvailability(bool available)
        {

        }
        public void UpdatePrice(decimal price)
        {

        }
        public void AddStock(int quantity)
        {

        }
        public void RemoveStock(int quantity)
        {

        }

    }
}
