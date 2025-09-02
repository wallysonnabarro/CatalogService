using CatalogService.Models;
using CatalogService.Models.Resultados;
using System.Numerics;

namespace CatalogService.Data
{
    public interface IProdutosRepository : IGenericRepository<Produto>
    {
        Task<Result<Produto>> ProdutoPorId(Guid id);
        Task<Result<bool>> AtualizarSituacaoProduto(Guid id, SituacaoEnum status);
        Task AtualizarQuantidadeProdutos(ICollection<ProdutoAtualizarQuantidade> lista);
        Task<Result<Paginacao<Produto>>> ProdutosPaginado(PageWrapper wrapper);
        Task<Result<ICollection<ProdutosListaModels>>> ValidarListaProdutosAsync(ICollection<Guid> lista);
        Task<Result<Guid>> AdicionarProduto(ProdutoModel model);
    }
}
