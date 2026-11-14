namespace Application.Models
{
    public class CreateStockEntryDto
    {
        public decimal Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;

        public int? ProductId { get; set; }
        public int? IngredientId { get; set; }
    }
}