using Application.Interfaces;
using Application.Models.Categories;
using Domain.Entities;
using Domain.Interfaces;
namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        
        private readonly IGenericRepository<Category> _categoryRepository; //BUSCAR MODIFICAR ELIMINAR CATEGORIA
        private readonly IGenericRepository<Product> _productRepository;// COMPROBAR SI EXISTEN PRODUCTOS ASOCIADOS ANTES DE ELIMINAR UNA CATEEGORIA

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
            {
                return null;
            }

            return new CategoryDto
            {
                Id = category.CategoryId,
                Name = category.CategoryName,
                Description = category.CategoryDescription
            };
        }


       
           
    public Task<CategoryDto> CreateAsync(CreateCategoryDto categoryDto)
    {
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

        return Task.FromResult(result);
    }

        

        
        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto categoryDto)
        {
            var category = await _categoryRepository.GetIdAsync(id);

            if (category == null)
            {
                return false;
            }

            category.ChangeName(categoryDto.Name);

            _categoryRepository.Update(category);

            return true;
        }


       
        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetIdAsync(id);

            if (category == null)
            {
                return false;
            }

            var products = await _productRepository.GetByFilterAsync(
                product => product.CategoryId == id
            );

            if (products != null)
            {
                return false;
            }

            _categoryRepository.Remove(category);

            return true;
        }

    }
}