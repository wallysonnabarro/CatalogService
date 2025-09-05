using CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data
{
    public class ContextDb : DbContext
    {
        public ContextDb(DbContextOptions<ContextDb> options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<ApplicationLog> ApplicationLogs { get; set; }
    }
}
