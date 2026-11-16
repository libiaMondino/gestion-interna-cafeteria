using System;

namespace Domain.Entities
{
    public class OrderDetail
    {
        // Propiedades
        public int Id { get; set; }

        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal SubTotal { get; private set; }

        public int KitchenOrderId { get; set; }

        public int ProductId { get; set; }

        // Propiedades de navegación
        public KitchenOrder KitchenOrder { get; set; } = null!;

        public Product Product { get; set; } = null!;

        // Constructor para crear un detalle
        public OrderDetail(int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "La cantidad debe ser mayor que cero.");

            if (unitPrice < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(unitPrice),
                    "El precio unitario no puede ser negativo.");

            Quantity = quantity;
            UnitPrice = unitPrice;

            CalculateSubtotal();
        }

        // Constructor para Entity Framework
        private OrderDetail()
        {
        }

        // Calcula el subtotal
        private void CalculateSubtotal()
        {
            SubTotal = Quantity * UnitPrice;
        }
    }
}
