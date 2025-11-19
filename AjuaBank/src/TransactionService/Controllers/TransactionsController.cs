using Microsoft.AspNetCore.Mvc;
using TransactionService.Data;
using TransactionService.Models;

namespace TransactionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        [HttpPost("send")]
        public IActionResult Send([FromBody] Transaction tx)
        {
            tx.Id = System.Guid.NewGuid();
            tx.Status = "Pending";
            tx.CreatedAt = System.DateTime.UtcNow;
            return CreatedAtAction(nameof(GetById), new { id = tx.Id }, tx);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(System.Guid id) => Ok(new { id, status = "Pending" });
    }
}