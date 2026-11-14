using Application.Models;

namespace Application.Interfaces
{
    public interface IStockMovementService
    {
        Task<List<StockMovementDto>> GetAllAsync();

        Task<StockMovementDto?> GetByIdAsync(int id);

        Task<List<StockMovementDto>> GetByIngredientIdAsync(int ingredientId);

        Task<List<StockMovementDto>> GetByProductIdAsync(int productId);

        Task<StockMovementDto> RegisterEntryAsync(
            CreateStockEntryDto stockEntryDto,
            int userId);
    }
}