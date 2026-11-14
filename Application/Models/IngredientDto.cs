namespace Application.Models.Ingredients
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal QuantityStock { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
        public decimal MinimumStock { get; set; }
        public bool IsBelowMinimum { get; set; }
    }
}
