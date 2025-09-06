using OAuthServices.Aplicacao.Command;
using OAuthServices.Models;

namespace OAuthServices.Aplicacao
{
    public interface ICadastroUsuarioUseCase : IUseCaseExecute<NovoUsuarioModel, Guid>
    {
    }
}
