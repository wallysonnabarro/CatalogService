namespace OAuthServices.Models
{
    public class TokenRequest
    {
        public string client_id { get; set; } = string.Empty;
        public string client_secret { get; set; } = string.Empty;
    }
}
