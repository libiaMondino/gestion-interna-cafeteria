using Application.Models;

namespace Application.Interfaces
{
    public interface IRecipeDetailService
    {
        Task<List<RecipeDetailDto>> GetAllAsync();

        Task<RecipeDetailDto?> GetByIdAsync(int id);

        Task<List<RecipeDetailDto>> GetByProductIdAsync(int productId);

        Task<RecipeDetailDto> CreateAsync(
            CreateRecipeDetailDto recipeDetailDto);

        Task<bool> UpdateAsync(
            int id,
            UpdateRecipeDetailDto recipeDetailDto);

        Task<bool> DeleteAsync(int id);
    }
}