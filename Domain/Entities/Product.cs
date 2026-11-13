using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
    public class Product
    {
        // Propiedades
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? ProductDescription { get; set; }

        public decimal ProductPrice { get; set; }

        public ProductType ProductType { get; set; }

        public bool ProductIsAvailable { get; set; }

        public int? ProductQuantityStock { get; set; }

        public int? ProductMinimumStock { get; set; }

        public int CategoryId { get; set; }

        // Propiedades de navegación
        public Category Category { get; set; } = null!;

        public ICollection<OrderDetail> OrderDetails { get; set; }
            = new List<OrderDetail>();

        // Métodos

        public void ChangeAvailability(bool available)
        {
            ProductIsAvailable = available;
        }

        public void UpdatePrice(decimal price)
        {
            if (price < 0)
                throw new ArgumentException(
                    "El precio no puede ser negativo.");

            ProductPrice = price;
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException(
                    "La cantidad a agregar debe ser mayor que cero.");

            if (ProductQuantityStock == null)
                throw new InvalidOperationException(
                    "Este producto no controla stock.");

            ProductQuantityStock += quantity;
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException(
                    "La cantidad a retirar debe ser mayor que cero.");

            if (ProductQuantityStock == null)
                throw new InvalidOperationException(
                    "Este producto no controla stock.");

            if (ProductQuantityStock < quantity)
                throw new InvalidOperationException(
                    "No hay suficiente stock disponible.");

            ProductQuantityStock -= quantity;
        }
    }
}
