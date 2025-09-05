using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data
{
    public class ContextDb : DbContext
    {
        public ContextDb(DbContextOptions<ContextDb> options) : base(options)
        {
        }

        public DbSet<OrdermServico> OrdermServicos { get; set; }
        public DbSet<Produtos> ProdutosVendidos { get; set; }
        public DbSet<ApplicationLog> ApplicationLogs { get; set; }
    }
}
