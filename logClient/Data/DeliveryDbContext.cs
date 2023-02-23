using Microsoft.EntityFrameworkCore;
using logClient.Models;

namespace logClient.Data
{
    public class DeliveryDbContext : DbContext
    {
        public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : base(options) { }

        public DbSet<Delivery> Deliveries { get; set; }
    }
}
