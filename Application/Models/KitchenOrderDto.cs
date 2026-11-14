using Domain.Enums;

namespace Application.Models
{
    public class KitchenOrderDto
    {
        public int Id { get; set; }
        public DateTime? SentDateTime { get; set; }
        public KitchenOrderStatus Status { get; set; }
        public int OrderId { get; set; }

        public List<OrderDetailDto> OrderDetails { get; set; }
            = new List<OrderDetailDto>();
    }
}