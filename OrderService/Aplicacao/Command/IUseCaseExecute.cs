using OrderService.Models.Resultados;

namespace OrderService.Aplicacao.Command
{
    public interface IUseCaseExecute<TInput, TOutput>
    {
        Task<Result<TOutput>> Executar(TInput input);
    }
}
