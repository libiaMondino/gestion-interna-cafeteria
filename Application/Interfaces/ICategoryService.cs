
using Application.Models.Categories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();

        Task<CategoryDto?> GetByIdAsync(int id);

        Task<CategoryDto> CreateAsync(CreateCategoryDto categoryDto);

        Task<bool> UpdateAsync(int id, UpdateCategoryDto categoryDto);

        Task<bool> DeleteAsync(int id);
    }
}
