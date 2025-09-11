using System.ComponentModel.DataAnnotations;

namespace WorkerOrdem.Models
{
    public class Produtos
    {
        public Guid Id { get; set; }
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
    }
}
