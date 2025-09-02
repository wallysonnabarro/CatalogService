using OrderService.Models;

namespace OrderService.Data
{
    public interface IOrdemServicoRepository : IGenericRepository<OrdermServico>
    {
        Task<Guid> GravarOrdemServiço(OrdermServico orderm);
    }
}
