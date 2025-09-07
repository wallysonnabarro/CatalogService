using System.Text.Json.Serialization;

namespace Web.Models
{
    public class AccessToken
    {
        [JsonPropertyName("access_token")]
        public string Access_token { get; set; }
    }
}
