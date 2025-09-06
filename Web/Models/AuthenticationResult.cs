namespace Web.Models
{
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public List<CookieInfo> Cookies { get; set; }
    }
}
