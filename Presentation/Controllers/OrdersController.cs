using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/orders
        [HttpGet]
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();

            return Ok(orders);
        }

        // GET: api/orders/5
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que cero.");

            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
                return NotFound("No se encontró el pedido.");

            return Ok(order);
        }

        // GET: api/orders/active/table/3
        [HttpGet("active/table/{tableId:int}")]
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<IActionResult> GetActiveByTableId(int tableId)
        {
            if (tableId <= 0)
                return BadRequest("El ID de la mesa debe ser mayor que cero.");

            var order = await _orderService.GetActiveByTableIdAsync(tableId);

            if (order == null)
                return NotFound("La mesa no tiene un pedido activo.");

            return Ok(order);
        }

        // POST: api/orders
        [HttpPost]
        [Authorize(Roles = "Waiter")]
        public async Task<IActionResult> Create(
            [FromBody] CreateOrderDto orderDto)
        {
            if (!int.TryParse(User.FindFirstValue("sub"), out int userId)
                || userId <= 0)
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            var order = await _orderService.CreateAsync(orderDto, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = order.Id },
                order
            );
        }

        // PATCH: api/orders/5/close
        [HttpPatch("{id:int}/close")]
        [Authorize(Roles = "Waiter")]
        public async Task<IActionResult> Close(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que cero.");

            var order = await _orderService.CloseAsync(id);

            return Ok(order);
        }
    }
}
