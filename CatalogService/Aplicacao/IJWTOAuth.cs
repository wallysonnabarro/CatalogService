using CatalogService.Models;

namespace CatalogService.Aplicacao
{
    public interface IJWTOAuth
    {
        Task<string> GetToken();
    }
}
