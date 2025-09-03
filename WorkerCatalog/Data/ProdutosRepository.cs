using Microsoft.EntityFrameworkCore;
using WorkerCatalog.Models;
using WorkerCatalog.Models.Resultados;

namespace WorkerCatalog.Data
{
    public class ProdutosRepository : GenericRepository<Produto>, IProdutosRepository
    {
        public ProdutosRepository(ContextDb context) : base(context)
        {

        }

        public async Task AtualizarQuantidadeProdutos(ICollection<ProdutoAtualizarQuantidade> lista)
        {
            try
            {
                foreach (var item in lista)
                {
                    var produto = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == item.ProdutoId);

                    if (produto != null)
                    {
                        produto.QuantidadeEstoque -= item.Quantidade;
                        Update(produto);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Result<bool>> AtualizarSituacaoProduto(Guid id, SituacaoEnum status)
        {
            try
            {
                var produto = await GetByIdAsync(id);
                produto.Status = status.ToString();
                return Result<bool>.Success;
            }
            catch (Exception e)
            {
                return Result<bool>.Failed(new Errors { Description = e.Message });
            }
        }
    }
}
