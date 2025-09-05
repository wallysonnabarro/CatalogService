using Microsoft.EntityFrameworkCore;
using OAuthServices.Models;

namespace OAuthServices.Data
{
    public class LogContextDb : DbContext
    {
        public LogContextDb(DbContextOptions<LogContextDb> options) : base(options)
        {
        }

        public DbSet<ApplicationLog> ApplicationLogs { get; set; }
    }
}
