using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;// agregamos esto para que pruct name inicie con una cadena vacia, lo cual no quiere decir que permitamos valores nuos
        public string? ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public ProductType ProductType { get; set; }
        public bool ProductIsAvailable { get; set; }
        public int? ProductQuantityStock { get; set; }
        public int? ProductMinimumStock { get; set; }
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
