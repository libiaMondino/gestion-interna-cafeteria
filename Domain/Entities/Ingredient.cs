using System;
using Domain.Exceptions;

namespace Domain.Entities
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; private set; } = string.Empty;
        public decimal QuantityStock { get; private set; }
        public string UnitOfMeasure { get; private set; } = string.Empty;
        public decimal MinimumStock { get; private set; }

        public ICollection<RecipeDetail> RecipeDetails { get; set; } = new List<RecipeDetail>();

        private Ingredient() { }

        public Ingredient(
            string name,
            string unitOfMeasure,
            decimal minimumStock)
        {
            ChangeName(name);
            ChangeUnitOfMeasure(unitOfMeasure);
            ChangeMinimumStock(minimumStock);
        }

        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "El nombre del insumo no puede estar vacío.");
            }

            if (name.Trim().Length > 100)
            {
                throw new ArgumentException(
                    "El nombre del insumo no puede superar los 100 caracteres.");
            }

            Name = name.Trim();
        }

        public void ChangeUnitOfMeasure(string unitOfMeasure)
        {
            if (string.IsNullOrWhiteSpace(unitOfMeasure))
            {
                throw new ArgumentException(
                    "La unidad de medida no puede estar vacía.");
            }

            if (unitOfMeasure.Trim().Length > 50)
            {
                throw new ArgumentException(
                    "La unidad de medida no puede superar los 50 caracteres.");
            }

            if (UnitOfMeasure == unitOfMeasure.Trim())
            {
                return;
            }

            if (QuantityStock > 0)
            {
                throw new BusinessRuleException(
                    "No se puede cambiar la unidad de medida mientras haya stock.");
            }

            UnitOfMeasure = unitOfMeasure.Trim();
        }

        public void ChangeMinimumStock(decimal minimumStock)
        {
            if (minimumStock < 0)
            {
                throw new ArgumentException(
                    "El stock mínimo no puede ser negativo.");
            }

            MinimumStock = minimumStock;
        }

        public void RegisterStockEntry(decimal quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException(
                    "La cantidad de entrada debe ser mayor que cero.");
            }

            QuantityStock += quantity;
        }

        public void RegisterStockExit(decimal quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException(
                    "La cantidad de salida debe ser mayor que cero.");
            }

            if (!HasSufficientStock(quantity))
            {
                throw new BusinessRuleException(
                    "No hay stock suficiente.");
            }

            QuantityStock -= quantity;
        }

        public bool HasSufficientStock(decimal quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity),
                    "La cantidad debe ser mayor que cero.");
            }

            return QuantityStock >= quantity;
        }

        public bool IsBelowMinimum()
        {
            return QuantityStock <= MinimumStock;
        }
    }
}
