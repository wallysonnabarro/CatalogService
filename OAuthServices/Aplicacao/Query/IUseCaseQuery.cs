using OAuthServices.Models.Resultados;

namespace OAuthServices.Aplicacao.Query
{
    public interface IUseCaseQuery<TInput, TOutput>
    {
        Task<Result<TOutput>> Query(TInput input);
    }
}
