using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("register")]
        public IActionResult Register([FromBody] object req) => Created("", new { message = "registered" });

        [HttpPost("login")]
        public IActionResult Login([FromBody] object req) => Ok(new { accessToken = "TODO" });
    }
}