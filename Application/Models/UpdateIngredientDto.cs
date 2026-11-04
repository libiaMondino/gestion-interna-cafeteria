namespace Application.Models
{
    public class UpdateIngredientDto
    {
        public string Name { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;
        public decimal MinimumStock { get; set; }
    }
}