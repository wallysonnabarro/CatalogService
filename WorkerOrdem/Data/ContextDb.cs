using Microsoft.EntityFrameworkCore;
using WorkerOrdem.Models;

namespace WorkerOrdem.Data
{
    public class ContextDb : DbContext
    {
        public ContextDb(DbContextOptions<ContextDb> options) : base(options)
        {
        }

        public DbSet<OrdermServico> OrdermServicos { get; set; }
        public DbSet<Produtos> ProdutosVendidos { get; set; }
    }
}
