using Application.Interfaces;
using Application.Models.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _productService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateProductDto productDto)
        {
            var product = await _productService.CreateAsync(productDto);
            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateProductDto productDto)
        {
            if (!await _productService.UpdateAsync(id, productDto))
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/availability")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeAvailability(int id, ChangeProductAvailabilityDto productDto)
        {
            if (!await _productService.ChangeAvailabilityAsync(id, productDto.ProductIsAvailable!.Value))
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/price")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePrice(int id, UpdateProductPriceDto productDto)
        {
            if (!await _productService.UpdatePriceAsync(id, productDto.ProductPrice!.Value))
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/stock/add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddStock(int id, ProductStockQuantityDto productDto)
        {
            try
            {
                if (!await _productService.AddStockAsync(id, productDto.Quantity))
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException exception) when (
                exception.Message == "Este producto no controla stock.")
            {
                return Conflict(new { error = exception.Message });
            }
        }

        [HttpPatch("{id}/stock/remove")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveStock(int id, ProductStockQuantityDto productDto)
        {
            try
            {
                if (!await _productService.RemoveStockAsync(id, productDto.Quantity))
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException exception) when (
                exception.Message == "Este producto no controla stock." ||
                exception.Message == "No hay suficiente stock disponible.")
            {
                return Conflict(new { error = exception.Message });
            }
        }
    }
}
