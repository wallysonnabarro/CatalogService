using OrderService.Models;

namespace OrderService.Servico
{
    public interface IOrdemServicoServices
    {
        Task<List<ProdutosListaModels>> ValidarListaProdutos(List<Guid> lista);
        Task AtualizarQuantidadeProdutos(List<Produto> lista);
    }
}
