using Application.Models.Products;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto productDto);
        Task<bool> UpdateAsync(int id, UpdateProductDto productDto);
        Task<bool> ChangeAvailabilityAsync(int id, bool available);
        Task<bool> UpdatePriceAsync(int id, decimal price);
        Task<bool> AddStockAsync(int id, int quantity);
        Task<bool> RemoveStockAsync(int id, int quantity);
    }
}
