using OAuthServices.Aplicacao.Command;

namespace OAuthServices.Aplicacao
{
    public interface IValidarEmailUseCase : IUseCaseExecute<string, bool>
    {
    }
}
