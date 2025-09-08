using System.Text.Json.Serialization;

namespace Web.Models
{
    public class OrderGeradaModel
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
    }
}
