using Microsoft.AspNetCore.Mvc;

namespace FraudDetectionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FraudController : ControllerBase
    {
        [HttpPost("score")]
        public IActionResult Score([FromBody] object req) => Ok(new { score = 0.1, flagged = false });
    }
}