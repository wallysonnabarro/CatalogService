namespace WorkerCatalog.Models
{
    public class ProdutoModel
    {
        public required string Nome { get; set; }
        public decimal Valor { get; set; }
        public int QuantidadeEstoque { get; set; }
    }
}
