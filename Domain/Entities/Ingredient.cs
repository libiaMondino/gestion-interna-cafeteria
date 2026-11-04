using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal QuantityStock { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty; // evaluar si enum

        public decimal MinimumStock { get; set; }

        public void RegisterStockEntry(decimal quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("La cantidad de entrada debe ser mayor que cero.");
            }

            QuantityStock += quantity;
        }
        public void RegisterStockExit(decimal quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("La cantidad de salida debe ser mayor que cero.");
            }

            if (!HasSufficientStock(quantity))
            {
                throw new InvalidOperationException("No hay stock suficiente.");
            }

            QuantityStock -= quantity;
        }
        public bool HasSufficientStock(decimal quantity)
        {
            return QuantityStock >= quantity;
        }
        public bool IsBelowMinimum()
        {
            return QuantityStock <=  MinimumStock;
        }
    }
}
