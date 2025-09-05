using CatalogService.Models;
using CatalogService.Models.Resultados;
using CatalogService.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CatalogService.Data
{
    public class ProdutosRepository : GenericRepository<Produto>, IProdutosRepository
    {
        private readonly ICorrelationLogger _logger;

        public ProdutosRepository(ContextDb context, ICorrelationLogger logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<Result<Guid>> AdicionarProduto(ProdutoModel model)
        {
            try
            {
                _logger.LogInformation("Buscando produto por ID: {ProdutoId}", JsonSerializer.Serialize(model));

                var existe = await _context.Produtos.AnyAsync(x => x.Nome.ToLower() == model.Nome.ToLower());

                if (existe)
                {
                    _logger.LogInformation("Já existe um produto cadastrado com esse nome.", model.Nome);

                    return Result<Guid>.Failed(new Errors { Description = "Já existe um produto cadastrado com esse nome." });
                }

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
                _logger.LogError(e, "Erro ao adicionar um novo produto.", JsonSerializer.Serialize(model));
                return Result<Guid>.Failed(new Errors { Description = e.Message });
            }
        }

        public async Task AtualizarQuantidadeProdutos(ICollection<ProdutoAtualizarQuantidade> lista)
        {
            try
            {
                _logger.LogInformation("Persistir atualização da quantidade do produto.", JsonSerializer.Serialize(lista));

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
                _logger.LogError(e, "Erro ao atualizar a quantidade do produto.", JsonSerializer.Serialize(lista));
                throw new Exception(e.Message);
            }
        }

        public async Task<Result<bool>> AtualizarSituacaoProduto(Guid id, SituacaoEnum status)
        {
            try
            {
                _logger.LogInformation($"Persistir atualizar situação do produto. {status.ToString()}", id);

                var produto = await GetByIdAsync(id);
                produto.Status = status.ToString();
                return Result<bool>.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro ao atulizar a situação do produto. {status.ToString()}", id);
                return Result<bool>.Failed(new Errors { Description = e.Message });
            }
        }

        public async Task<Result<Produto>> ProdutoPorId(Guid id)
        {
            try
            {
                _logger.LogInformation($"Buscar produto por id", id);

                return Result<Produto>.Sucesso(await GetByIdAsync(id));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro ao buscar produto por id.", id);
                return Result<Produto>.Failed(new Errors { Description = e.Message });
            }
        }

        public async Task<Result<Paginacao<Produto>>> ProdutosPaginado(PageWrapper wrapper)
        {
            try
            {
                _logger.LogInformation($"Buscar produto produtos paginado", wrapper.Page);

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
                _logger.LogError(e, $"Erro ao buscar produto paginados.", wrapper.Page);
                return Result<Paginacao<Produto>>.Failed(new Errors { Description = e.Message });
            }
        }

        public async Task<Result<ICollection<ProdutosListaModels>>> ValidarListaProdutosAsync(ICollection<Guid> lista)
        {
            try
            {
                _logger.LogInformation($"Busca lista de produtos.", JsonSerializer.Serialize(lista));

                List<ProdutosListaModels> nova = new List<ProdutosListaModels>();

                foreach (var id in lista)
                {
                    var produto = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == id);
                    if (produto != null && produto.Status.Equals(SituacaoEnum.Ativo.ToString()))
                    {
                        nova.Add(new ProdutosListaModels { Id = produto.Id, Valor = produto.Valor });
                    }
                }

                return Result<ICollection<ProdutosListaModels>>.Sucesso(nova);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro ao buscar lista de produtos.", JsonSerializer.Serialize(lista));
                return Result<ICollection<ProdutosListaModels>>.Failed(new Errors { Description = e.Message });
            }
        }
    }
}
