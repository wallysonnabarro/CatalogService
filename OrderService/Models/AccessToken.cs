using System.Text.Json.Serialization;

namespace OrderService.Models
{
    public class AccessToken
    {
        [JsonPropertyName("access_token")]
        public required string Access_token { get; set; }
    }
}
