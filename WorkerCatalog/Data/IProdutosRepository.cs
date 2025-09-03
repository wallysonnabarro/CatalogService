using WorkerCatalog.Models;
using WorkerCatalog.Models.Resultados;

namespace WorkerCatalog.Data
{
    public interface IProdutosRepository : IGenericRepository<Produto>
    {
        Task<Result<bool>> AtualizarSituacaoProduto(Guid id, SituacaoEnum status);
        Task AtualizarQuantidadeProdutos(ICollection<ProdutoAtualizarQuantidade> lista);
    }
}
