
using System;
using Domain.Enums;

namespace Domain.Entities
{
    public class StockMovement
    {
        public int Id { get; set; }
        public DateTime DateTime { get; private set; }
        public decimal Quantity { get; private set; }
        public StockMovementType MovementType { get; private set; }
        public string Reason { get; private set; } = string.Empty;

        // Foreign keys
        public int UserId { get; set; }
        public int? ProductId { get; set; }
        public int? IngredientId { get; set; }
        public int? OrderDetailId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Product? Product { get; set; }
        public Ingredient? Ingredient { get; set; }
        public OrderDetail? OrderDetail { get; set; }

        private StockMovement() { }

        public StockMovement(
            decimal quantity,
            StockMovementType movementType,
            string reason,
            int userId,
            int? productId = null,
            int? ingredientId = null,
            int? orderDetailId = null)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "La cantidad debe ser mayor que cero.");

            if (!Enum.IsDefined(typeof(StockMovementType), movementType))
                throw new ArgumentException(
                    "El tipo de movimiento no es válido.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException(
                    "El motivo del movimiento es obligatorio.");

            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            if (productId.HasValue == ingredientId.HasValue)
                throw new ArgumentException(
                    "El movimiento debe corresponder a un producto o a un ingrediente.");

            if (productId.HasValue && productId.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(productId));

            if (ingredientId.HasValue && ingredientId.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(ingredientId));

            if (orderDetailId.HasValue && orderDetailId.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(orderDetailId));

            DateTime = DateTime.Now;
            Quantity = quantity;
            MovementType = movementType;
            Reason = reason.Trim();

            UserId = userId;
            ProductId = productId;
            IngredientId = ingredientId;
            OrderDetailId = orderDetailId;
        }
    }
}
