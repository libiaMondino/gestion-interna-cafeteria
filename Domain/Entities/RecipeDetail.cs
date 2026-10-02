using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class RecipeDetail
    {
        public int Id { get; set; }
        public decimal RequiredQuantity { get; set; }
        public int ProductId { get; set; }
        public int IngredientId { get; set; }

        //Métodos
        public decimal CalculateConsumption(int units) {
            return units; //No es así el método, es solo para que no lance error por falta de return
        }
    }
}
