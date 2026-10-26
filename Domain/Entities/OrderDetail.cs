namespace Domain.Entities
{
    public class OrderDetail
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal SubTotal { get; set; }

        public int KitchenOrderId { get; set; }

        public int ProductId { get; set; }

        // Calcula el subtotal del detalle
        public decimal CalculateSubtotal()
        {
            if (Quantity <= 0)
                throw new System.InvalidOperationException(
                    "La cantidad debe ser mayor que cero.");

            if (UnitPrice < 0)
                throw new System.InvalidOperationException(
                    "El precio unitario no puede ser negativo.");

            return UnitPrice * Quantity;
        }
    }
}
