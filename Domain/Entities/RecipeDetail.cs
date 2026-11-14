using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class RecipeDetail
    {
        public int Id { get; set; }
        public decimal RequiredQuantity { get; private set; }
        public int ProductId { get; private set; }
        public int IngredientId { get; private set; }

        public Product Product { get; set; } = null!;
        public Ingredient Ingredient { get; set; } = null!;

        private RecipeDetail() { }

        public RecipeDetail(
            int productId,
            int ingredientId,
            decimal requiredQuantity)
        {
            if (productId <= 0)
            {
                throw new ArgumentException(
                    "El identificador del producto debe ser mayor que cero.");
            }

            if (ingredientId <= 0)
            {
                throw new ArgumentException(
                    "El identificador del insumo debe ser mayor que cero.");
            }

            ProductId = productId;
            IngredientId = ingredientId;
            ChangeRequiredQuantity(requiredQuantity);
        }

        //Métodos
        public void ChangeRequiredQuantity(decimal requiredQuantity)
        {
            if (requiredQuantity <= 0)
            {
                throw new ArgumentException(
                    "La cantidad necesaria del insumo debe ser mayor que cero.");
            }

            RequiredQuantity = requiredQuantity;
        }
        public decimal CalculateConsumption(int units)
        {
            if (units <= 0)
            {
                throw new ArgumentException(
                    "La cantidad de unidades debe ser mayor que cero.");
            }

            return RequiredQuantity * units;
        }
    }
}
