using System.Collections.Generic;

namespace Application.Models
{
    public class CreateKitchenOrderDto
    {
        public int OrderId { get; set; }

        public List<CreateOrderDetailDto> OrderDetails { get; set; }
            = new List<CreateOrderDetailDto>();
    }
}