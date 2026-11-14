using System.Security.Claims;
using Application.Interfaces;
using Application.Models;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/stock-movements")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class StockMovementsController : ControllerBase
    {
        private readonly IStockMovementService _stockMovementService;

        public StockMovementsController(
            IStockMovementService stockMovementService)
        {
            _stockMovementService = stockMovementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movements = await _stockMovementService.GetAllAsync();

            return Ok(movements);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movement = await _stockMovementService.GetByIdAsync(id);

            if (movement == null)
            {
                return NotFound();
            }

            return Ok(movement);
        }

        [HttpGet("ingredient/{ingredientId:int}")]
        public async Task<IActionResult> GetByIngredientId(int ingredientId)
        {
            var movements =
                await _stockMovementService.GetByIngredientIdAsync(
                    ingredientId);

            return Ok(movements);
        }

        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var movements =
                await _stockMovementService.GetByProductIdAsync(productId);

            return Ok(movements);
        }

        [HttpPost("entries")]
        public async Task<IActionResult> RegisterEntry(
            [FromBody] CreateStockEntryDto stockEntryDto)
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                return Unauthorized(new
                {
                    error = "El token no contiene un identificador de usuario válido."
                });
            }

            var movement = await _stockMovementService.RegisterEntryAsync(
                stockEntryDto,
                userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = movement.Id },
                movement);
        }
    }
}