using Microsoft.EntityFrameworkCore;
using WorkerCatalog.Models;

namespace WorkerCatalog.Data
{
    public class ContextDb : DbContext
    {
        public ContextDb(DbContextOptions<ContextDb> options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
    }
}
