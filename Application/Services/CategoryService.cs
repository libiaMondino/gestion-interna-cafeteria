using System.Linq;
using Application.Interfaces;
using Application.Models.Categories;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<Product> _productRepository;

        public CategoryService(
            IGenericRepository<Category> categoryRepository,
            IGenericRepository<Product> productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return categories.Select(category => new CategoryDto
            {
                Id = category.CategoryId,
                Name = category.CategoryName,
                Description = category.CategoryDescription
            }).ToList();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetIdAsync(id);

            if (category == null)
                return null;

            return new CategoryDto
            {
                Id = category.CategoryId,
                Name = category.CategoryName,
                Description = category.CategoryDescription
            };
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto categoryDto)
        {
            var existingCategory = await _categoryRepository.GetByFilterAsync(
                category => category.CategoryName == categoryDto.Name.Trim()
            );

            if (existingCategory != null)
                throw new ArgumentException("Ya existe una categoría con ese nombre.");

            var category = new Category(categoryDto.Name)
            {
                CategoryDescription = categoryDto.Description
            };

            _categoryRepository.Add(category);

            var result = new CategoryDto
            {
                Id = category.CategoryId,
                Name = category.CategoryName,
                Description = category.CategoryDescription
            };

            return result;
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateCategoryDto categoryDto)
        {
            var category = await _categoryRepository.GetIdAsync(id);

            if (category == null)
                return false;

            var existingCategory = await _categoryRepository.GetByFilterAsync(
                category => category.CategoryName == categoryDto.Name.Trim()
                    && category.CategoryId != id
            );

            if (existingCategory != null)
                throw new ArgumentException("Ya existe una categoría con ese nombre.");

            category.ChangeName(categoryDto.Name);
            category.CategoryDescription = categoryDto.Description;

            _categoryRepository.Update(category);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetIdAsync(id);

            if (category == null)
                return false;

            var products = await _productRepository.GetListByFilterAsync(
                product => product.CategoryId == id
            );

            if (products.Any())
                return false;

            _categoryRepository.Remove(category);

            return true;
        }
    }
}
