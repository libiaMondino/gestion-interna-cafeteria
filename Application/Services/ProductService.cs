using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using Application.Models.Products;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Category> _categoryRepository;

        public ProductService(
            IGenericRepository<Product> productRepository,
            IGenericRepository<Category> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var categories = (await _categoryRepository.GetAllAsync())
                .ToDictionary(category => category.CategoryId);

            return products.Select(product => MapToDto(product,
                categories.TryGetValue(product.CategoryId, out var category)
                    ? category.CategoryName : string.Empty)).ToList();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            ValidateId(id);
            var product = await _productRepository.GetIdAsync(id);
            if (product == null)
                return null;

            var category = await _categoryRepository.GetIdAsync(product.CategoryId);
            return MapToDto(product, category?.CategoryName ?? string.Empty);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto productDto)
        {
            ValidateDto(productDto);
            var category = await GetCategoryAsync(productDto.CategoryId);
            var product = new Product
            {
                ProductName = productDto.ProductName.Trim(),
                ProductDescription = productDto.ProductDescription,
                ProductType = productDto.ProductType,
                ProductQuantityStock = productDto.ProductQuantityStock,
                ProductMinimumStock = productDto.ProductMinimumStock,
                CategoryId = category.CategoryId
            };
            product.UpdatePrice(productDto.ProductPrice);
            product.ChangeAvailability(productDto.ProductIsAvailable);
            _productRepository.Add(product);
            return MapToDto(product, category.CategoryName);
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductDto productDto)
        {
            ValidateId(id);
            ValidateDto(productDto);
            var product = await _productRepository.GetIdAsync(id);
            if (product == null)
                return false;

            var category = await GetCategoryAsync(productDto.CategoryId);
            product.ProductName = productDto.ProductName.Trim();
            product.ProductDescription = productDto.ProductDescription;
            product.CategoryId = category.CategoryId;
            _productRepository.Update(product);
            return true;
        }

        public async Task<bool> ChangeAvailabilityAsync(int id, bool available)
        {
            ValidateId(id);
            var product = await _productRepository.GetIdAsync(id);
            if (product == null)
                return false;

            product.ChangeAvailability(available);
            _productRepository.Update(product);
            return true;
        }

        public async Task<bool> UpdatePriceAsync(int id, decimal price)
        {
            ValidateId(id);
            var product = await _productRepository.GetIdAsync(id);
            if (product == null)
                return false;

            product.UpdatePrice(price);
            _productRepository.Update(product);
            return true;
        }

        public async Task<bool> AddStockAsync(int id, int quantity)
        {
            ValidateId(id);
            var product = await _productRepository.GetIdAsync(id);
            if (product == null)
                return false;

            product.AddStock(quantity);
            _productRepository.Update(product);
            return true;
        }

        public async Task<bool> RemoveStockAsync(int id, int quantity)
        {
            ValidateId(id);
            var product = await _productRepository.GetIdAsync(id);
            if (product == null)
                return false;

            product.RemoveStock(quantity);
            _productRepository.Update(product);
            return true;
        }

        private async Task<Category> GetCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetIdAsync(id);
            if (category == null)
                throw new ArgumentException("La categoría indicada no existe.");

            return category;
        }

        private static void ValidateId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El id del producto debe ser mayor que cero.");
        }

        private static void ValidateDto(object dto)
        {
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, new ValidationContext(dto), results, true))
                throw new ArgumentException(string.Join(" ", results.Select(result => result.ErrorMessage)));
        }

        private static ProductDto MapToDto(Product product, string categoryName)
        {
            return new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice,
                ProductType = product.ProductType,
                ProductIsAvailable = product.ProductIsAvailable,
                ProductQuantityStock = product.ProductQuantityStock,
                ProductMinimumStock = product.ProductMinimumStock,
                CategoryId = product.CategoryId,
                CategoryName = categoryName
            };
        }
    }
}
