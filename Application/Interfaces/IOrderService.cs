using Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllAsync();

        Task<OrderDto?> GetByIdAsync(int id);

        Task<OrderDto> CreateAsync(CreateOrderDto orderDto);

        Task<bool> CloseAsync(int id);
    }
}
