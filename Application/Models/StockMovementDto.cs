using Domain.Enums;

namespace Application.Models
{
    public class StockMovementDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Quantity { get; set; }
        public StockMovementType MovementType { get; set; }
        public string Reason { get; set; } = string.Empty;

        public int UserId { get; set; }
        public int? ProductId { get; set; }
        public int? IngredientId { get; set; }
        public int? OrderDetailId { get; set; }
    }
}