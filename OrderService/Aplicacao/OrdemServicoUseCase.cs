using OrderService.Data;
using OrderService.Models;
using OrderService.Models.Resultados;
using OrderService.Servico;

namespace OrderService.Aplicacao
{
    public class OrdemServicoUseCase(IOrdemServicoServices _ordemServicoServices, IOrdemServicoRepository _OrdemRepository) : IOrdemServicoUseCase
    {
        public async Task<Result<Guid>> Executar(List<Produto> input)
        {
            try
            {
                //Validar a lista de produtos na api CatalogService
                var listaProdutosValidos = await _ordemServicoServices.ValidarListaProdutos(input.Select(p => p.ProdutoId).ToList());

                if (listaProdutosValidos.Count == 0) return Result<Guid>.Failed(new Errors { Description = "Lista de produtos inválida" });

                var novaLista = input.Where(x => listaProdutosValidos.Any(p => p.Id == x.ProdutoId))
                .Select(x => new Produtos
                {
                    ProdutoId = x.ProdutoId,
                    Quantidade = x.Quantidade,
                    Valor = listaProdutosValidos.First(p => p.Id == x.ProdutoId).Valor
                })
                .OrderBy(x => x.ProdutoId)
                .ToList();

                if (novaLista.Count == 0) return Result<Guid>.Failed(new Errors { Description = "Lista de produtos inválida" });

                var novaOrdem = new OrdermServico
                {
                    Id = Guid.NewGuid(),
                    DataHoraRegistro = DateTime.UtcNow,
                    Produtos = novaLista,
                    Status = OrdemServicoSituacaoEnum.Aberta.ToString(),
                    Total = novaLista.Sum(p => p.Valor * p.Quantidade)
                };

                var id = await _OrdemRepository.GravarOrdemServiço(novaOrdem);

                // Atualizar a quantidade dos produtos na api CatalogService
                await _ordemServicoServices.AtualizarQuantidadeProdutos(input);

                return Result<Guid>.Sucesso(id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failed(new Errors { Description = e.Message });
            }
        }
    }
}
