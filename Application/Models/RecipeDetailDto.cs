namespace Application.Models
{
    public class RecipeDetailDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int IngredientId { get; set; }
        public decimal RequiredQuantity { get; set; }
    }
}