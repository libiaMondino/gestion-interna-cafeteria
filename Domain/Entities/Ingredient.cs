
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; private set; } = string.Empty;
        public decimal QuantityStock { get; private set; }
        public string UnitOfMeasure { get; private set; } = string.Empty;
        public decimal MinimumStock { get; private set; }

        public ICollection<RecipeDetail> RecipeDetails { get; set; }
            = new List<RecipeDetail>();

        private Ingredient() { }

        public Ingredient(string name, string unitOfMeasure,
            decimal minimumStock)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "El nombre del ingrediente es obligatorio.");

            if (string.IsNullOrWhiteSpace(unitOfMeasure))
                throw new ArgumentException(
                    "La unidad de medida es obligatoria.");

            if (minimumStock < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(minimumStock),
                    "El stock mínimo no puede ser negativo.");

            Name = name.Trim();
            UnitOfMeasure = unitOfMeasure.Trim();
            MinimumStock = minimumStock;
            QuantityStock = 0;
        }

        public void RegisterStockEntry(decimal quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "La cantidad ingresada debe ser mayor que cero.");

            QuantityStock += quantity;
        }

        public void RegisterStockExit(decimal quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "La cantidad retirada debe ser mayor que cero.");

            if (!HasSufficientStock(quantity))
                throw new InvalidOperationException(
                    "No hay stock suficiente para realizar la salida.");

            QuantityStock -= quantity;
        }

        public bool HasSufficientStock(decimal quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "La cantidad debe ser mayor que cero.");

            return QuantityStock >= quantity;
        }

        public bool IsBelowMinimum()
        {
            return QuantityStock < MinimumStock;
        }
    }
}
