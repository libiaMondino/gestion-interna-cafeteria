using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Models.Users;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto userDto)
        {
            var user = await _userService.CreateAsync(userDto);

            return CreatedAtAction(nameof(GetById), new { id = user.UserId }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserDto userDto)
        {
            var updated = await _userService.UpdateAsync(id, userDto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var activated = await _userService.ActivateAsync(id);

            if (!activated)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var deactivated = await _userService.DeactivateAsync(id);

            if (!deactivated)
                return NotFound();

            return NoContent();
        }
    }
}
