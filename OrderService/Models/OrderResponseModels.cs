using System.Text.Json.Serialization;

namespace OrderService.Models
{
    public class OrdemServicoListResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("dataHoraRegistro")]
        public DateTime DataHoraRegistro { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("total")]
        public decimal Total { get; set; }
        [JsonPropertyName("quantidadeProdutos")]
        public int QuantidadeProdutos { get; set; }
    }

    public class OrdemDetalhesResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("dataHoraRegistro")]
        public DateTime DataHoraRegistro { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("total")]
        public decimal Total { get; set; }
        [JsonPropertyName("produtos")]
        public List<ProdutoOrdemResponse> Produtos { get; set; } = new List<ProdutoOrdemResponse>();
    }

    public class ProdutoOrdemResponse
    {
        [JsonPropertyName("produtoId")]
        public Guid ProdutoId { get; set; }
        [JsonPropertyName("nomeProduto")]
        public string NomeProduto { get; set; } = string.Empty;
        [JsonPropertyName("quantidade")]
        public int Quantidade { get; set; }
        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
    }
}