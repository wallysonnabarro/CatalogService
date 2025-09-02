namespace OrderService.Models
{
    public class Produto
    {
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
    }
}