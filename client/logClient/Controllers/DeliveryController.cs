using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using logClient.Data;
using logClient.Models;

namespace logClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly DeliveryDbContext _context;

        public DeliveryController(DeliveryDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Delivery>> Get()
        {
            return await _context.Deliveries.ToListAsync();
        }

        [HttpGet("id")]
        [ProducesResponseType(typeof(Delivery), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);

            return delivery == null ? NotFound() : Ok(delivery);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendRequest(Delivery delivery)
        {
            await _context.Deliveries.AddAsync(delivery);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = delivery.ID }, delivery);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deliveryToDelete = await _context.Deliveries.FindAsync(id);
            if (deliveryToDelete == null) return NotFound();

            _context.Deliveries.Remove(deliveryToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
