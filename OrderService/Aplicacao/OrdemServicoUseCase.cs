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

                if (listaProdutosValidos == null) return Result<Guid>.Failed(new Errors { Description = "Lista de produtos inválida" });

                var novaLista = input.Where(x => listaProdutosValidos.Contains(x.ProdutoId))
                    .OrderBy(x => x.ProdutoId)
                    .ToList();

                if (novaLista == null) return Result<Guid>.Failed(new Errors { Description = "Lista de produtos inválida" });

                //gerar a ordem de servico
                var novaOrdem = new OrdermServico
                {
                    Id = Guid.NewGuid(),
                    DataHoraRegistro = DateTime.UtcNow,
                    Produtos = novaLista,
                    Status = OrdemServicoSituacaoEnum.Aberta.ToString(),
                    Total = novaLista.Sum(p => p.Valor * p.Quantidade)
                };

                var id = await _OrdemRepository.GravarOrdemServiço(novaOrdem);

                //retornar o guid da ordem de servico
                return Result<Guid>.Sucesso(id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failed(new Errors { Description = e.Message });
            }
        }
    }
}
