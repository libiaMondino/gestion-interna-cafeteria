namespace Application.Models
{
    public class RecipeDetailDto
    {
        public int Id { get; set; }

        public decimal RequiredQuantity { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public int IngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;

        public string UnitOfMeasure { get; set; } = string.Empty;
    }
}
