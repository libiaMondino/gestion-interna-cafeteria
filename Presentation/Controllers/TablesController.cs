using Application.Interfaces;
using Application.Models.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TablesController : ControllerBase
    {
        private readonly ITableService _service;
        public TablesController(ITableService service)
        {
            _service = service;
        }
        // GET: api/tables
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableDto>>> GetAll()
        {
            var tables = await _service.GetAllAsync();
            return Ok(tables);
        }

        // GET /api/tables/search?num=5
        [HttpGet("search")]
        public async Task<ActionResult<TableDto>> GetByNumberAsync( [FromQuery] int num)
        {
                if (num <= 0)
                {
                    return BadRequest("El número de mesa debe ser mayor a cero");
                }
                var table = await _service.GetByNumberAsync(num);
                if (table == null)
                {
                    return NotFound($"No se encontró la mesa con el número {num}");
                }
                return Ok(table);
        }
    }
}
