namespace CatalogService.Models
{
    public class Produto
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public decimal Valor { get; set; }
        public int QuantidadeEstoque { get; set; }
        public string Status { get; set; } = SituacaoEnum.Ativo.ToString();
    }
}
