using System.Numerics;

namespace CatalogService.Models
{
    public class ProdutoAtualizarQuantidade
    {
        public Guid Id { get; set; }
        public int Quantidade { get; set; }
    }
}
