using System;
using Domain.Enums;

namespace Domain.Entities
{
    public class StockMovement
    {
        public int Id { get; private set; }
        public DateTime DateTime { get; private set; }
        public decimal Quantity { get; private set; }
        public StockMovementType MovementType { get; private set; }
        public string Reason { get; private set; } = string.Empty;

        public int UserId { get; private set; }
        public int? ProductId { get; private set; }
        public int? IngredientId { get; private set; }
        public int? OrderDetailId { get; private set; }

        public StockMovement(
            decimal quantity,
            StockMovementType movementType,
            string reason,
            int userId,
            int? productId,
            int? ingredientId,
            int? orderDetailId)
        {
            Quantity = quantity;
            MovementType = movementType;
            Reason = reason?.Trim() ?? string.Empty;
            UserId = userId;
            ProductId = productId;
            IngredientId = ingredientId;
            OrderDetailId = orderDetailId;
            DateTime = System.DateTime.UtcNow;

            if (!Validate())
            {
                throw new ArgumentException(
                    "Los datos del movimiento de stock no son válidos.");
            }
        }

        public bool Validate()
        {
            if (Quantity <= 0 || UserId <= 0)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(Reason) || Reason.Length > 255)
            {
                return false;
            }

            if (MovementType != StockMovementType.In &&
                MovementType != StockMovementType.Out)
            {
                return false;
            }

            if (ProductId.HasValue == IngredientId.HasValue)
            {
                return false;
            }

            if (ProductId.HasValue && ProductId.Value <= 0)
            {
                return false;
            }

            if (IngredientId.HasValue && IngredientId.Value <= 0)
            {
                return false;
            }

            if (MovementType == StockMovementType.In &&
                OrderDetailId.HasValue)
            {
                return false;
            }

            if (MovementType == StockMovementType.Out &&
                (!OrderDetailId.HasValue || OrderDetailId.Value <= 0))
            {
                return false;
            }

            return true;
        }
    }
}