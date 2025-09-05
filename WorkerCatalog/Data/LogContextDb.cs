using Microsoft.EntityFrameworkCore;
using WorkerCatalog.Models;

namespace WorkerCatalog.Data
{
    public class LogContextDb : DbContext
    {
        public LogContextDb(DbContextOptions<LogContextDb> options) : base(options)
        {
        }

        public DbSet<ApplicationLog> ApplicationLogs { get; set; }
    }
}
