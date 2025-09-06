using Web.Models.Resultados;

namespace Web.Aplicacao.Command
{
    public interface IUseCaseExecute<TInput, TOutput>
    {
        Task<Result<TOutput>> Executar(TInput input);
    }
}
