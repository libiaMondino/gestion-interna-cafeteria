using Application.Interfaces;
using Application.Models.Categories;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        public Task<List<CategoryDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CategoryDto?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryDto> CreateAsync(CreateCategoryDto categoryDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, UpdateCategoryDto categoryDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}