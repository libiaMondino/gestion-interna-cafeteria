using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Enums;

namespace Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public string Status { get; set; } = "Activo";

        public decimal Total { get; private set; }

        public int TableId { get; set; }

        public int UserId { get; set; }

        // Propiedades de navegación
        public Table Table { get; set; } = null!;

        public User User { get; set; } = null!;

        public ICollection<KitchenOrder> KitchenOrders { get; set; }
            = new List<KitchenOrder>();

        // Agrega una comanda al pedido
        public void AddKitchenOrder(KitchenOrder kitchenOrder)
        {
            if (Status != "Activo")
                throw new InvalidOperationException(
                    "No se pueden agregar comandas a un pedido cerrado.");

            if (kitchenOrder == null)
                throw new ArgumentNullException(nameof(kitchenOrder));

            if (kitchenOrder.Status != KitchenOrderStatus.Pending)
                throw new InvalidOperationException(
                    "Solo se pueden agregar comandas pendientes.");

            kitchenOrder.OrderId = Id;

            KitchenOrders.Add(kitchenOrder);

            CalculateTotal();
        }

        // Calcula el total excluyendo las comandas canceladas
        public void CalculateTotal()
        {
            Total = KitchenOrders
                .Where(k => k.Status != KitchenOrderStatus.Cancelled)
                .SelectMany(k => k.OrderDetails)
                .Sum(d => d.CalculateSubtotal());
        }

        // Cierra el pedido
        public void Close()
        {
            if (Status != "Activo")
                throw new InvalidOperationException(
                    "El pedido ya está cerrado.");

            bool hasPendingKitchenOrders = KitchenOrders.Any(k =>
                k.Status != KitchenOrderStatus.Delivered &&
                k.Status != KitchenOrderStatus.Cancelled);

            if (hasPendingKitchenOrders)
                throw new InvalidOperationException(
                    "No se puede cerrar el pedido porque hay comandas que no fueron entregadas ni canceladas.");

            CalculateTotal();

            Status = "Cerrado";
        }
    }
}
