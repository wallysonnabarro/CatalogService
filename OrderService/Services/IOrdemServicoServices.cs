using OrderService.Models;

namespace OrderService.Services
{
    public interface IOrdemServicoServices
    {
        Task<List<ProdutosListaModels>> ValidarListaProdutos(List<Guid> lista);
        Task AtualizarQuantidadeProdutos(List<Produto> lista, Guid idOrdem);
    }
}
