using OAuthServices.Models.Resultados;

namespace OAuthServices.Aplicacao.Command
{
    public interface IUseCaseExecute<TInput, TOutput>
    {
        Task<Result<TOutput>> Executar(TInput input);
    }
}
