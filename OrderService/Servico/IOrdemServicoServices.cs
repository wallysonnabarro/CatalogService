using OrderService.Models;

namespace OrderService.Servico
{
    public interface IOrdemServicoServices
    {
        Task<List<Guid>> ValidarListaProdutos(List<Guid> lista);
    }
}
