namespace OAuthServices.Aplicacao
{
    public interface IJWTOAuth
    {
        Task<string> GetToken();
    }
}
