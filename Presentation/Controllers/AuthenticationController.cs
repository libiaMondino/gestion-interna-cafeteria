using Application.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _service;
        public AuthenticationController( IAuthenticationService service)
        {
            _service = service;
        }
        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate( [FromBody] CredentialsLoginDto credentials)
        {
            string token = await _service.Authenticate(credentials);
            return Ok(token);
        }
    }
}
