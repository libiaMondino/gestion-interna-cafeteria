using Application.Interfaces;
using Application.Models.Tables;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles ="Admin,Waiter")]
        public async Task<ActionResult<IEnumerable<TableDto>>> GetAll()
        {
            var tables = await _service.GetAllAsync();
            return Ok(tables);
        }

        // GET /api/tables/search?num=5
        [HttpGet("search")]
        [Authorize(Roles ="Admin,Waiter")]
        public async Task<ActionResult<TableDto>> GetByNumberAsync([FromQuery] int num)
        {
            if (num <= 0)
            {
                return BadRequest("El número de mesa debe ser mayor a cero");
            }
            var table = await _service.GetByNumberAsync(num);
            
            return Ok(table);
        }

        // GET /api/tables/occupied
        [HttpGet("occupied")]
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<ActionResult<IEnumerable<TableDto>>> GetOccupiedTablesAsync()
        {
            var tables = await _service.GetOccupiedTablesAsync();
            return Ok(tables);
        }

        // GET /api/tables/released
        [HttpGet("released")]
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<ActionResult<IEnumerable<TableDto>>> GetReleasedTablesAsync()
        {
            var tables = await _service.GetReleasedTablesAsync();
            return Ok(tables);
        }

        // POST
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddAsync([FromBody] CreateTableDto objModel)
        {
            // Validaciones
            if (objModel is null)
                return BadRequest($"Es necesario llenar todos los campos de la mesa");
            if (objModel.Number <= 0 || objModel.Capacity <= 0)
                return BadRequest("El número de la mesa y su capacidad deben ser mayores a cero");
            await _service.AddAsync(objModel);
            return Created();

        }

        // PUT /api/tables/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] UpdateTableDto objModel)
        {
            // Validaciones
            if (objModel is null)
                return BadRequest($"Es necesario llenar todos los campos de la mesa con id {id}");
            if (id <= 0)
                return BadRequest("El id debe ser mayor a cero");
            if (objModel.Number <= 0 || objModel.Capacity <= 0)
                return BadRequest("El número de la mesa y su capacidad deben ser mayores a cero");

            await _service.UpdateAsync(id, objModel);
            return NoContent();
        }

        // DELETE /api/tables/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> RemoveAsync([FromRoute] int id)
        {
            if (id <= 0)
                return BadRequest("El id debe ser mayor a cero");
            await _service.RemoveAsync(id); 
            return NoContent();
        }

        // PUT /api/tables/{id}/status/occupied
        [HttpPut("{id}/status/occupied")]
        [Authorize(Roles ="Waiter")]
        public async Task<ActionResult> OccupyAsync([FromRoute] int id)
        {
            if (id <= 0)
                return BadRequest("El id debe ser mayor a cero");
            await _service.OccupyAsync(id);
            return NoContent(); 
        }

        // PUT /api/tables/{id}/status/released
        [HttpPut("{id}/status/released")]
        [Authorize(Roles ="Waiter")]
        public async Task<ActionResult> ReleaseAsync([FromRoute]int id)
        {
            if (id <= 0)
                return BadRequest("El id debe ser mayor a cero");
            await _service.ReleaseAsync(id);
            return NoContent();
        }

       
    }
}
