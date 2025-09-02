using CatalogService.Models;
using CatalogService.Models.Resultados;
using System.Numerics;

namespace CatalogService.Data
{
    public interface IProdutosRepository : IGenericRepository<Produto>
    {
        Task<Result<Produto>> ProdutoPorId(Guid id);
        Task<Result<bool>> AtualizarSituacaoProduto(Guid id, SituacaoEnum status);
        /// <summary>
        /// Retornar uma lista de produtos invalidos a partir de uma lista fornecida, caso houver.
        /// </summary>
        /// <param name="lista">ProdutoAtualizarQuantidade</param>
        /// <returns></returns>
        Task<Result<ICollection<ProdutoAtualizarQuantidade>>> AtualizarQuantidadeProdutos(ICollection<ProdutoAtualizarQuantidade> lista);
        Task<Result<Paginacao<Produto>>> ProdutosPaginado(PageWrapper wrapper);
        Task<Result<ICollection<Guid>>> ValidarListaProdutosAsync(ICollection<Guid> lista);
        Task<Result<Guid>> AdicionarProduto(ProdutoModel model);
    }
}
