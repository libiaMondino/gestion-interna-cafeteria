using Application.Models;

namespace Application.Interfaces
{
    public interface IIngredientService
    {
        Task<List<IngredientDto>> GetAllAsync();

        Task<IngredientDto?> GetByIdAsync(int id);

        Task<IngredientDto> CreateAsync(CreateIngredientsDto ingredientDto);

        Task<bool> UpdateAsync(int id, UpdateIngredientDto ingredientDto);

        Task<bool> DeleteAsync(int id);
    }
}