namespace Application.Models
{
    public class CreateRecipeDetailDto
    {
        public int ProductId { get; set; }
        public int IngredientId { get; set; }
        public decimal RequiredQuantity { get; set; }
    }
}