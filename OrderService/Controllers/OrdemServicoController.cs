using Microsoft.AspNetCore.Mvc;
using OrderService.Aplicacao;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers
{
    [Route("orders/orderns")]
    [ApiController]
    public class OrdemServicoController(IOrdemServicoUseCase _useCase, ICorrelationLogger _logger) : ControllerBase
    {
        [HttpPost]
        [Route("gerar-ordem")]
        public async Task<IActionResult> GerarOrdemServico(List<Produto> produtos)
        {
            _logger.LogInformation("Iniciando geração de ordem de serviço para {QuantidadeProdutos} produtos", produtos.Count);

            try
            {
                var guid = await _useCase.Executar(produtos);

                if(guid == null) return BadRequest("Erro ao gerar ordem de serviço.");

                _logger.LogInformation("Ordem de serviço gerada com sucesso. IDs gerados: {QuantidadeIds}", guid.Dados.Id);
                return Ok(guid.Dados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar ordem de serviço para {QuantidadeProdutos} produtos", produtos.Count);
                throw;
            }
        }
    }
}
