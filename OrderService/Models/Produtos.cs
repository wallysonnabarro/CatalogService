
using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class Produtos
    {
        [Key]
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
    }
}
