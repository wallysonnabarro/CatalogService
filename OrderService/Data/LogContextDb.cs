using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data
{
    public class LogContextDb : DbContext
    {
        public LogContextDb(DbContextOptions<LogContextDb> options) : base(options)
        {
        }

        public DbSet<ApplicationLog> ApplicationLogs { get; set; }
    }
}
