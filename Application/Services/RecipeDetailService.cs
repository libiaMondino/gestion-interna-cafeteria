using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services
{
    public class RecipeDetailService : IRecipeDetailService
    {
        private readonly IGenericRepository<RecipeDetail> _recipeRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Ingredient> _ingredientRepository;

        public RecipeDetailService(
            IGenericRepository<RecipeDetail> recipeRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<Ingredient> ingredientRepository)
        {
            _recipeRepository = recipeRepository;
            _productRepository = productRepository;
            _ingredientRepository = ingredientRepository;
        }

        public async Task<List<RecipeDetailDto>> GetAllAsync()
        {
            var details = await _recipeRepository.GetAllAsync();
            var result = new List<RecipeDetailDto>();

            foreach (var detail in details)
            {
                result.Add(await MapToDtoAsync(detail));
            }

            return result;
        }

        public async Task<RecipeDetailDto?> GetByIdAsync(int id)
        {
            var detail = await _recipeRepository.GetIdAsync(id);

            if (detail == null)
            {
                return null;
            }

            return await MapToDtoAsync(detail);
        }

        public async Task<List<RecipeDetailDto>> GetByProductIdAsync(
            int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException(
                    "El identificador del producto debe ser mayor que cero.");
            }

            var details = await _recipeRepository.GetListByFilterAsync(
                r => r.ProductId == productId);

            var result = new List<RecipeDetailDto>();

            foreach (var detail in details)
            {
                result.Add(await MapToDtoAsync(detail));
            }

            return result;
        }

        private async Task<RecipeDetailDto> MapToDtoAsync(
            RecipeDetail detail)
        {
            var product = await _productRepository.GetIdAsync(
                detail.ProductId);

            var ingredient = await _ingredientRepository.GetIdAsync(
                detail.IngredientId);

            if (product == null || ingredient == null)
            {
                throw new InvalidOperationException(
                    "El detalle de receta tiene una relación inexistente.");
            }

            return new RecipeDetailDto
            {
                Id = detail.Id,
                ProductId = detail.ProductId,
                ProductName = product.ProductName,
                IngredientId = detail.IngredientId,
                IngredientName = ingredient.Name,
                UnitOfMeasure = ingredient.UnitOfMeasure,
                RequiredQuantity = detail.RequiredQuantity
            };
        }

        public async Task<RecipeDetailDto> CreateAsync(
            CreateRecipeDetailDto recipeDetailDto)
        {
            var detail = new RecipeDetail(
                recipeDetailDto.ProductId,
                recipeDetailDto.IngredientId,
                recipeDetailDto.RequiredQuantity
            );

            var product = await _productRepository.GetIdAsync(detail.ProductId);

            if (product == null)
            {
                throw new ArgumentException(
                    "El producto indicado no existe.");
            }

            if (product.ProductType != ProductType.Prepared)
            {
                throw new BusinessRuleException(
                    "Solo los productos elaborados pueden tener receta.");
            }

            var ingredient = await _ingredientRepository.GetIdAsync(
                detail.IngredientId);

            if (ingredient == null)
            {
                throw new ArgumentException(
                    "El insumo indicado no existe.");
            }

            var existingDetail = await _recipeRepository.GetByFilterAsync(
                r => r.ProductId == detail.ProductId &&
                     r.IngredientId == detail.IngredientId);

            if (existingDetail != null)
            {
                throw new BusinessRuleException(
                    "El insumo ya forma parte de la receta de este producto.");
            }

            _recipeRepository.Add(detail);

            return new RecipeDetailDto
            {
                Id = detail.Id,
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                IngredientId = ingredient.Id,
                IngredientName = ingredient.Name,
                UnitOfMeasure = ingredient.UnitOfMeasure,
                RequiredQuantity = detail.RequiredQuantity
            };
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateRecipeDetailDto recipeDetailDto)
        {
            var detail = await _recipeRepository.GetIdAsync(id);

            if (detail == null)
            {
                return false;
            }

            var product = await _productRepository.GetIdAsync(detail.ProductId);

            if (product == null)
            {
                throw new InvalidOperationException(
                    "El producto asociado a la receta no existe.");
            }

            if (product.ProductType != ProductType.Prepared)
            {
                throw new BusinessRuleException(
                    "Solo los productos elaborados pueden tener receta.");
            }

            detail.ChangeRequiredQuantity(recipeDetailDto.RequiredQuantity);

            _recipeRepository.Update(detail);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var detail = await _recipeRepository.GetIdAsync(id);

            if (detail == null)
            {
                return false;
            }

            var product = await _productRepository.GetIdAsync(detail.ProductId);

            if (product == null)
            {
                throw new InvalidOperationException(
                    "El producto asociado a la receta no existe.");
            }

            if (product.ProductType == ProductType.Prepared &&
                product.ProductIsAvailable)
            {
                var detailCount = await _recipeRepository.FilteredCountAsync(
                    r => r.ProductId == detail.ProductId);

                if (detailCount <= 1)
                {
                    throw new BusinessRuleException(
                        "No se puede eliminar el último insumo de la receta " +
                        "mientras el producto esté disponible para la venta.");
                }
            }

            _recipeRepository.Remove(detail);

            return true;
        }
    }
}