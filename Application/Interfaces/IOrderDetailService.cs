using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;

namespace Application.Interfaces
{
    public interface IOrderDetailService
    {
        Task<List<OrderDetailDto>> GetAllAsync();

        Task<OrderDetailDto?> GetByIdAsync(int id);

        Task<List<OrderDetailDto>> GetByKitchenOrderIdAsync(
            int kitchenOrderId);
    }
}
