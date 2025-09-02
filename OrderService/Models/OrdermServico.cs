namespace OrderService.Models
{
    public class OrdermServico
    {
        public Guid Id { get; set; }
        public DateTime DataHoraRegistro { get; set; }
        public string Status { get; set; } = OrdemServicoSituacaoEnum.Aberta.ToString();
        public decimal Total { get; set; }
        public required List<Produtos> Produtos { get; set; }
    }
}
