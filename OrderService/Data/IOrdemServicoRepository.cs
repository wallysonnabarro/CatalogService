using OrderService.Models;

namespace OrderService.Data
{
    public interface IOrdemServicoRepository : IGenericRepository<OrdermServico>
    {
        Task<Guid> GravarOrdemServiço(OrdermServico orderm);
        Task<List<OrdermServico>> ListarOrdensAsync();
        Task<OrdermServico?> ObterOrdemPorIdAsync(Guid id);
        Task<OrderPageResponse<OrdermServico>> ListarOrdensPaginadasAsync(OrderPageRequest request);
    }
}
