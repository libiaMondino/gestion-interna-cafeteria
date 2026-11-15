
using System;

namespace Domain.Entities
{
    public class RecipeDetail
    {
        public int Id { get; set; }
        public decimal RequiredQuantity { get; private set; }

        public int ProductId { get; set; }
        public int IngredientId { get; set; }

        // Navigation properties
        public Product Product { get; set; } = null!;
        public Ingredient Ingredient { get; set; } = null!;

        private RecipeDetail() { }

        public RecipeDetail(
            decimal requiredQuantity,
            int productId,
            int ingredientId)
        {
            if (requiredQuantity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(requiredQuantity),
                    "La cantidad requerida debe ser mayor que cero.");

            if (productId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(productId),
                    "El producto no es válido.");

            if (ingredientId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(ingredientId),
                    "El ingrediente no es válido.");

            RequiredQuantity = requiredQuantity;
            ProductId = productId;
            IngredientId = ingredientId;
        }

        public decimal CalculateConsumption(int units)
        {
            if (units <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(units),
                    "La cantidad de productos debe ser mayor que cero.");

            return RequiredQuantity * units;
        }
    }
}
