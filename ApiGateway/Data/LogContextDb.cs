using Microsoft.EntityFrameworkCore;
using ApiGateway.Models;

namespace ApiGateway.Data
{
    public class LogContextDb : DbContext
    {
        public LogContextDb(DbContextOptions<LogContextDb> options) : base(options)
        {
        }

        public DbSet<ApplicationLog> ApplicationLogs { get; set; }
    }
}
