using System.Text.Json.Serialization;

namespace OrderService.Models
{
    public class ProdutosListaModels
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
    }
}
