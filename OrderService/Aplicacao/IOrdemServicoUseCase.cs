using OrderService.Aplicacao.Command;
using OrderService.Models;

namespace OrderService.Aplicacao
{
    public interface IOrdemServicoUseCase : IUseCaseExecute<List<Produto>, Guid>
    {
    }
}
