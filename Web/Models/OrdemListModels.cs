namespace Web.Models
{
    public class OrdemServicoListViewModel
    {
        public Guid Id { get; set; }
        public DateTime DataHoraRegistro { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int QuantidadeProdutos { get; set; }
    }

    public class OrdemDetalhesViewModel
    {
        public Guid Id { get; set; }
        public DateTime DataHoraRegistro { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<ProdutoOrdemViewModel> Produtos { get; set; } = new List<ProdutoOrdemViewModel>();
    }

    public class ProdutoOrdemViewModel
    {
        public Guid ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
        public decimal Subtotal => Valor * Quantidade;
    }
}
