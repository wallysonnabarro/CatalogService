using OrderService.Models.Resultados;

namespace OrderService.Aplicacao.Query
{
    public interface IUseCaseQuery<TInput, TOutput>
    {
        Task<Result<TOutput>> Query(TInput input);
    }
}
