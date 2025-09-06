using Web.Aplicacao.Command;
using Web.Models;

namespace Web.Services
{
    public interface ILoginServices : IUseCaseExecute<LoginModel, AuthenticationResult>
    {
    }
}
