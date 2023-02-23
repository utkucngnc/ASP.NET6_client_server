using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using logServer.Data;
using logServer.Models;
using Microsoft.Data.SqlClient;

namespace logServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly logDbContext _context;

        public LogController(logDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Log>> GetAll()
        {
            return await _context.Logs.ToListAsync();
        }

        [HttpGet("id")]
        [ProducesResponseType(typeof(Log), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await _context.Logs.FindAsync(id);

            return log == null ? NotFound() : Ok(log);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Log log)
        {
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = log.ID }, log);
        }

        [HttpPut("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, Log log)
        {
            if (id != log.ID)
            {
                return BadRequest();
            }

            _context.Entry(log).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var logToDelete = await _context.Logs.FindAsync(id);
            if (logToDelete == null) return NotFound();

            _context.Logs.Remove(logToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
