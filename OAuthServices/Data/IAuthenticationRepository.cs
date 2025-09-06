using OAuthServices.Data.Generico;
using OAuthServices.Models;
using OAuthServices.Models.Resultados;

namespace OAuthServices.Data
{
    public interface IAuthenticationRepository : IGenericRepository<Usuario>
    {
        Task<Result<Guid>> AuthenticateEmail(LoginModel model);
    }
}
