using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
    public class KitchenOrder
    {
        public int Id { get; set; }

        public DateTime? SentDateTime { get; set; }

        public KitchenOrderStatus Status { get; set; }
            = KitchenOrderStatus.Pending;

        public int OrderId { get; set; }

        // Propiedad de navegación
        public ICollection<OrderDetail> OrderDetails { get; set; }
            = new List<OrderDetail>();

        // Agrega un detalle a la comanda
        public void AddOrderDetail(OrderDetail orderDetail)
        {
            if (Status != KitchenOrderStatus.Pending)
                throw new InvalidOperationException(
                    "Solo se pueden agregar productos a una comanda pendiente.");

            if (orderDetail == null)
                throw new ArgumentNullException(nameof(orderDetail));

            if (orderDetail.Quantity <= 0)
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");

            if (orderDetail.UnitPrice < 0)
                throw new ArgumentException(
                    "El precio unitario no puede ser negativo.");

            orderDetail.SubTotal = orderDetail.CalculateSubtotal();

            OrderDetails.Add(orderDetail);
        }

        // Envía la comanda a cocina
        public void Send()
        {
            if (Status != KitchenOrderStatus.Pending)
                throw new InvalidOperationException(
                    "Solo se pueden enviar comandas pendientes.");

            if (OrderDetails.Count == 0)
                throw new InvalidOperationException(
                    "No se puede enviar una comanda sin productos.");

            SentDateTime = DateTime.Now;
            Status = KitchenOrderStatus.Sent;
        }

        // Marca la comanda como en preparación
        public void MarkAsPreparing()
        {
            if (Status != KitchenOrderStatus.Sent)
                throw new InvalidOperationException(
                    "La comanda debe estar enviada para comenzar su preparación.");

            Status = KitchenOrderStatus.InPreparation;
        }

        // Marca la comanda como lista
        public void MarkAsReady()
        {
            if (Status != KitchenOrderStatus.InPreparation)
                throw new InvalidOperationException(
                    "La comanda debe estar en preparación para marcarse como lista.");

            Status = KitchenOrderStatus.Ready;
        }

        // Registra la entrega de la comanda
        public void RegisterDelivery()
        {
            if (Status != KitchenOrderStatus.Ready)
                throw new InvalidOperationException(
                    "Solo se pueden entregar comandas que estén listas.");

            Status = KitchenOrderStatus.Delivered;
        }

        // Cancela la comanda
        public void Cancel()
        {
            if (Status == KitchenOrderStatus.Delivered)
                throw new InvalidOperationException(
                    "No se puede cancelar una comanda ya entregada.");

            if (Status == KitchenOrderStatus.Cancelled)
                throw new InvalidOperationException(
                    "La comanda ya está cancelada.");

            Status = KitchenOrderStatus.Cancelled;
        }
    }
}
