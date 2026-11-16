using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Application.Models
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }

        public int TableId { get; set; }
        public int UserId { get; set; }

        public List<KitchenOrderDto> KitchenOrders { get; set; }
            = new List<KitchenOrderDto>();
    }
}
