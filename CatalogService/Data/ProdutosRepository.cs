using CatalogService.Models;
using CatalogService.Models.Resultados;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data
{
    public class ProdutosRepository : GenericRepository<Produto>, IProdutosRepository
    {
        public ProdutosRepository(ContextDb context) : base(context)
        {

        }

        public async Task<Result<Guid>> AdicionarProduto(ProdutoModel model)
        {
            try
            {
                var existe = await _context.Produtos.AnyAsync(x => x.Nome.ToLower() == model.Nome.ToLower());

                if (existe)
                    return Result<Guid>.Failed(new Errors { Description = "Já existe um produto cadastrado com esse nome." });

                var produto = new Produto
                {
                    Nome = model.Nome,
                    Valor = model.Valor,
                    QuantidadeEstoque = model.QuantidadeEstoque,
                    Status = SituacaoEnum.Ativo.ToString()
                };

                await AddAsync(produto);
                return Result<Guid>.Sucesso(produto.Id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failed(new Errors { Description = e.Message });
            }
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

        public async Task<Result<Produto>> ProdutoPorId(Guid id)
        {
            try
            {
                return Result<Produto>.Sucesso(await GetByIdAsync(id));
            }
            catch (Exception e)
            {
                return Result<Produto>.Failed(new Errors { Description = e.Message });
            }
        }

        public async Task<Result<Paginacao<Produto>>> ProdutosPaginado(PageWrapper wrapper)
        {
            try
            {
                var quantidade = await CountAsync(expression: c => c.Status == SituacaoEnum.Ativo.ToString());

                var dados = await FindPagedAsync(
                predicate: x => x.Status == SituacaoEnum.Ativo.ToString(),
                skip: (wrapper.Page - 1) * wrapper.PageSize,
                take: wrapper.PageSize,
                null);

                var paginacao = new Paginacao<Produto>
                {
                    Count = quantidade,
                    Dados = dados.ToList(),
                    PageIndex = wrapper.Skip == 0 ? 1 : wrapper.Skip,
                    PageSize = wrapper.PageSize,
                    TotalPages = (int)Math.Ceiling((double)quantidade / wrapper.PageSize)
                };

                return Result<Paginacao<Produto>>.Sucesso(paginacao);
            }
            catch (Exception e)
            {
                return Result<Paginacao<Produto>>.Failed(new Errors { Description = e.Message });
            }
        }

        public async Task<Result<ICollection<ProdutosListaModels>>> ValidarListaProdutosAsync(ICollection<Guid> lista)
        {
            try
            {
                List<ProdutosListaModels> nova = new List<ProdutosListaModels>();

                foreach (var id in lista)
                {
                    var produto = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == id);
                    if (produto != null && produto.Status.Equals(SituacaoEnum.Ativo.ToString()))
                    {
                        nova.Add(new ProdutosListaModels { Id = produto.Id, Valor = produto.Valor});
                    }
                }

                return Result<ICollection<ProdutosListaModels>>.Sucesso(nova);
            }
            catch (Exception e)
            {
                return Result<ICollection<ProdutosListaModels>>.Failed(new Errors { Description = e.Message });
            }
        }
    }
}
