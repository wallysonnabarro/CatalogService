using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data
{
    public class LogContextDb : DbContext
    {
        public LogContextDb(DbContextOptions<LogContextDb> options) : base(options)
        {
        }

        public DbSet<ApplicationLog> ApplicationLogs { get; set; }
    }
}