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
    public class KitchenOrderController : ControllerBase
    {
        private readonly IKitchenOrderService _kitchenOrderService;

        public KitchenOrderController(
            IKitchenOrderService kitchenOrderService)
        {
            _kitchenOrderService = kitchenOrderService;
        }

        // GET: api/kitchenorder
        [HttpGet]
        [Authorize(Roles = "Admin,Waiter,Kitchen")]
        public async Task<IActionResult> GetAll()
        {
            var kitchenOrders =
                await _kitchenOrderService.GetAllAsync();

            return Ok(kitchenOrders);
        }

        // GET: api/kitchenorder/5
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Waiter,Kitchen")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que cero.");

            var kitchenOrder =
                await _kitchenOrderService.GetByIdAsync(id);

            if (kitchenOrder == null)
                return NotFound("No se encontró la comanda.");

            return Ok(kitchenOrder);
        }

        // GET: api/kitchenorder/order/5
        [HttpGet("order/{orderId:int}")]
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            if (orderId <= 0)
                return BadRequest("El ID del pedido debe ser mayor que cero.");

            var kitchenOrders =
                await _kitchenOrderService.GetByOrderIdAsync(orderId);

            return Ok(kitchenOrders);
        }

        // POST: api/kitchenorder
        [HttpPost]
        [Authorize(Roles = "Waiter")]
        public async Task<IActionResult> Create(
            [FromBody] CreateKitchenOrderDto kitchenOrderDto)
        {
            var kitchenOrder =
                await _kitchenOrderService.CreateAsync(kitchenOrderDto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = kitchenOrder.Id },
                kitchenOrder
            );
        }

        // POST: api/kitchenorder/5/details
        [HttpPost("{id:int}/details")]
        [Authorize(Roles = "Waiter")]
        public async Task<IActionResult> AddOrderDetail(
            int id,
            [FromBody] CreateOrderDetailDto detailDto)
        {
            if (id <= 0)
                return BadRequest("El ID de la comanda debe ser mayor que cero.");

            var kitchenOrder =
                await _kitchenOrderService.AddOrderDetailAsync(id, detailDto);

            return Ok(kitchenOrder);
        }

        // PATCH: api/kitchenorder/5/send
        [HttpPatch("{id:int}/send")]
        [Authorize(Roles = "Waiter")]
        public async Task<IActionResult> Send(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que cero.");

            var kitchenOrder =
                await _kitchenOrderService.SendAsync(id);

            return Ok(kitchenOrder);
        }

        // PATCH: api/kitchenorder/5/prepare
        [HttpPatch("{id:int}/prepare")]
        [Authorize(Roles = "Kitchen")]
        public async Task<IActionResult> MarkAsPreparing(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que cero.");

            if (!int.TryParse(User.FindFirstValue("sub"), out int userId)
                || userId <= 0)
            {
                return Unauthorized(
                    "No se pudo identificar al usuario autenticado.");
            }

            var kitchenOrder =
                await _kitchenOrderService.MarkAsPreparingAsync(id, userId);

            return Ok(kitchenOrder);
        }

        // PATCH: api/kitchenorder/5/ready
        [HttpPatch("{id:int}/ready")]
        [Authorize(Roles = "Kitchen")]
        public async Task<IActionResult> MarkAsReady(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que cero.");

            var kitchenOrder =
                await _kitchenOrderService.MarkAsReadyAsync(id);

            return Ok(kitchenOrder);
        }

        // PATCH: api/kitchenorder/5/deliver
        [HttpPatch("{id:int}/deliver")]
        [Authorize(Roles = "Waiter")]
        public async Task<IActionResult> RegisterDelivery(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que cero.");

            var kitchenOrder =
                await _kitchenOrderService.RegisterDeliveryAsync(id);

            return Ok(kitchenOrder);
        }

        // PATCH: api/kitchenorder/5/cancel
        [HttpPatch("{id:int}/cancel")]
        [Authorize(Roles = "Waiter")]
        public async Task<IActionResult> Cancel(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que cero.");

            var kitchenOrder =
                await _kitchenOrderService.CancelAsync(id);

            return Ok(kitchenOrder);
        }
    }
}
