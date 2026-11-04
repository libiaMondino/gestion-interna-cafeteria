namespace Application.Models
{ 
    public class CreateIngredientsDto
    {
        public string Name { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;
        public decimal MinimumStock { get; set; }
    }
}