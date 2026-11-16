using Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllAsync();

        Task<OrderDto?> GetByIdAsync(int id);

        Task<OrderDto?> GetActiveByTableIdAsync(int tableId);

        Task<OrderDto> CreateAsync(
            CreateOrderDto orderDto,
            int userId);

        Task<OrderDto> CloseAsync(int id);
    }
}
