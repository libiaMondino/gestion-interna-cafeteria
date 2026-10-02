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

        public void RegisterStockEntry( decimal quantity) { }
        public void RegisterStockExit( decimal quantity) { }
        public bool HasSufficientStock(decimal quantity)
        {
            return true; // No es así el método, está puesto así para que no lance error por falta de return
        }
        public bool IsBelowMinimum()
        {
            return true;// No es así el método, está puesto así para que no lance error por falta de return
        }
    }
}
