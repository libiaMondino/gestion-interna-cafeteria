using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;
namespace Domain.Entities
{
    public class KitchenOrder
    {
        public int Id { get; set; }
        public DateTime? SentDateTime { get; set; }
        public KitchenOrderStatus Status { get; set; }
        public int OrderId { get; set; }
        
        // Métodos
        public void AddOrderDetail(OrderDetail orderDetail){}
        public void Send() { }
        public void MarkAsPreparing() { }
        public void MarkAsReady() { }
        public void RegisterDelivery() { }
        public void Cancel() { }

    }
}
