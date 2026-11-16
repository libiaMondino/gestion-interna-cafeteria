using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        // GET: api/OrderDetail
        [HttpGet]
        [Authorize(Roles = "Admin,Waiter,Kitchen")]
        public async Task<IActionResult> GetAll()
        {
            var details = await _orderDetailService.GetAllAsync();

            return Ok(details);
        }

        // GET: api/OrderDetail/5
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Waiter,Kitchen")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que cero.");

            var detail = await _orderDetailService.GetByIdAsync(id);

            if (detail == null)
                return NotFound("No se encontró el detalle solicitado.");

            return Ok(detail);
        }

        // GET: api/OrderDetail/kitchen-order/5
        [HttpGet("kitchen-order/{kitchenOrderId:int}")]
        [Authorize(Roles = "Admin,Waiter,Kitchen")]
        public async Task<IActionResult> GetByKitchenOrderId(int kitchenOrderId)
        {
            if (kitchenOrderId <= 0)
                return BadRequest("El ID de la comanda debe ser mayor que cero.");

            var details = await _orderDetailService
                .GetByKitchenOrderIdAsync(kitchenOrderId);

            return Ok(details);
        }
    }
}
