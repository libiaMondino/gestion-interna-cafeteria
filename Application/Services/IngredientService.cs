using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IGenericRepository<Ingredient> _repository;
        private readonly IGenericRepository<RecipeDetail> _recipeRepository;
        private readonly IGenericRepository<StockMovement> _movementRepository;

        public IngredientService(
            IGenericRepository<Ingredient> repository,
            IGenericRepository<RecipeDetail> recipeRepository,
            IGenericRepository<StockMovement> movementRepository)
        {
            _repository = repository;
            _recipeRepository = recipeRepository;
            _movementRepository = movementRepository;
        }

        public async Task<List<IngredientDto>> GetAllAsync()
        {
            var ingredients = await _repository.GetAllAsync();

            var result = new List<IngredientDto>();

            foreach (var ingredient in ingredients)
            {
                result.Add(new IngredientDto
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    QuantityStock = ingredient.QuantityStock,
                    UnitOfMeasure = ingredient.UnitOfMeasure,
                    MinimumStock = ingredient.MinimumStock,
                    IsBelowMinimum = ingredient.IsBelowMinimum()
                });
            }

            return result;
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            var ingredient = await _repository.GetIdAsync(id);

            if (ingredient == null)
            {
                return null;
            }

            return new IngredientDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                QuantityStock = ingredient.QuantityStock,
                UnitOfMeasure = ingredient.UnitOfMeasure,
                MinimumStock = ingredient.MinimumStock,
                IsBelowMinimum = ingredient.IsBelowMinimum()
            };
        }

        public Task<IngredientDto> CreateAsync(CreateIngredientsDto ingredientDto)
        {
            var ingredient = new Ingredient(
                ingredientDto.Name,
                ingredientDto.UnitOfMeasure,
                ingredientDto.MinimumStock
            );

            _repository.Add(ingredient);

            var result = new IngredientDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                QuantityStock = ingredient.QuantityStock,
                UnitOfMeasure = ingredient.UnitOfMeasure,
                MinimumStock = ingredient.MinimumStock,
                IsBelowMinimum = ingredient.IsBelowMinimum()
            };

            return Task.FromResult(result);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateIngredientDto ingredientDto)
        {
            var ingredient = await _repository.GetIdAsync(id);

            if (ingredient == null)
            {
                return false;
            }

            var validatedData = new Ingredient(
                ingredientDto.Name,
                ingredientDto.UnitOfMeasure,
                ingredientDto.MinimumStock
            );

            if (ingredient.UnitOfMeasure != validatedData.UnitOfMeasure)
            {
                if (ingredient.QuantityStock > 0)
                {
                    throw new InvalidOperationException(
                        "No se puede cambiar la unidad de medida mientras haya stock.");
                }

                var recipe = await _recipeRepository.GetByFilterAsync(
                    r => r.IngredientId == id);

                var movement = await _movementRepository.GetByFilterAsync(
                    m => m.IngredientId == id);

                if (recipe != null || movement != null)
                {
                    throw new InvalidOperationException(
                        "No se puede cambiar la unidad de medida de un insumo con recetas o movimientos asociados.");
                }
            }

            ingredient.ChangeName(validatedData.Name);
            ingredient.ChangeUnitOfMeasure(validatedData.UnitOfMeasure);
            ingredient.ChangeMinimumStock(validatedData.MinimumStock);

            _repository.Update(ingredient);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ingredient = await _repository.GetIdAsync(id);

            if (ingredient == null)
            {
                return false;
            }

            if (ingredient.QuantityStock > 0)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar un insumo que tiene stock.");
            }

            var recipe = await _recipeRepository.GetByFilterAsync(
                r => r.IngredientId == id);

            if (recipe != null)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar un insumo asociado a una receta.");
            }

            var movement = await _movementRepository.GetByFilterAsync(
                m => m.IngredientId == id);

            if (movement != null)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar un insumo con movimientos registrados.");
            }

            _repository.Remove(ingredient);

            return true;
        }
    }
}