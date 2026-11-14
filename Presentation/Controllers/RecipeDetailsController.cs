using Application.Interfaces;
using Application.Models;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/recipe-details")]
    [Authorize]
    public class RecipeDetailsController : ControllerBase
    {
        private readonly IRecipeDetailService _recipeDetailService;

        public RecipeDetailsController(
            IRecipeDetailService recipeDetailService)
        {
            _recipeDetailService = recipeDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var details = await _recipeDetailService.GetAllAsync();

            return Ok(details);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var detail = await _recipeDetailService.GetByIdAsync(id);

            if (detail == null)
            {
                return NotFound();
            }

            return Ok(detail);
        }

        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var details = await _recipeDetailService.GetByProductIdAsync(
                productId);

            return Ok(details);
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateRecipeDetailDto recipeDetailDto)
        {
            var detail = await _recipeDetailService.CreateAsync(
                recipeDetailDto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = detail.Id },
                detail);
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateRecipeDetailDto recipeDetailDto)
        {
            var updated = await _recipeDetailService.UpdateAsync(
                id,
                recipeDetailDto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _recipeDetailService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}