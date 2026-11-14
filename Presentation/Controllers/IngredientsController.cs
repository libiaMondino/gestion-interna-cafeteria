using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.Models;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/ingredients")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ingredients = await _ingredientService.GetAllAsync();

            return Ok(ingredients);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ingredient = await _ingredientService.GetByIdAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
        [FromBody] CreateIngredientsDto ingredientDto)
        {
            var ingredient = await _ingredientService.CreateAsync(ingredientDto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = ingredient.Id },
                ingredient);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateIngredientDto ingredientDto)
        {
            var updated = await _ingredientService.UpdateAsync(id, ingredientDto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _ingredientService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}