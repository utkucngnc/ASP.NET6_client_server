using Microsoft.EntityFrameworkCore;
using logServer.Models;

namespace logServer.Data
{
    public class logDbContext : DbContext
    {
        public logDbContext(DbContextOptions<logDbContext> options) : base(options) { }

        public DbSet<Log> Logs { get; set; }
    }
}
