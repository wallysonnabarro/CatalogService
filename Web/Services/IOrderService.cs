using Web.Models;
using Web.Models.Resultados;

namespace Web.Services
{
    public interface IOrderService
    {
        Task<Result<OrderGeradaModel>> GerarOrdemServico(List<ProdutoParaOrdem> produtos);
        Task<OrdensPaginadasViewModel> ListarOrdensPaginadasAsync(PageWrapper pageWrapper);
        Task<OrdemDetalhesViewModel?> ObterOrdemPorIdAsync(Guid id);
    }
}
