using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;

namespace Application.Interfaces
{
    public interface IKitchenOrderService
    {
        Task<List<KitchenOrderDto>> GetAllAsync();

        Task<KitchenOrderDto?> GetByIdAsync(int id);

        Task<List<KitchenOrderDto>> GetByOrderIdAsync(int orderId);

        Task<KitchenOrderDto> CreateAsync(
            CreateKitchenOrderDto kitchenOrderDto);

        Task<KitchenOrderDto> AddOrderDetailAsync(
            int kitchenOrderId,
            CreateOrderDetailDto detailDto);

        Task<KitchenOrderDto> SendAsync(int id);

        Task<KitchenOrderDto> MarkAsPreparingAsync(int id, int userId);

        Task<KitchenOrderDto> MarkAsReadyAsync(int id);

        Task<KitchenOrderDto> RegisterDeliveryAsync(int id);

        Task<KitchenOrderDto> CancelAsync(int id);
    }
}
