using Microsoft.AspNetCore.Mvc;
using OrderService.Aplicacao;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers
{
    [Route("orders/orderns")]
    [ApiController]
    public class OrdemServicoController(
        IOrdemServicoUseCase _useCase, 
        IOrdemServicoRepository _repository,
        IOrderMappingService _mappingService,
        ICorrelationLogger _logger) : ControllerBase
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

        [HttpPost]
        [Route("listar-ordens")]
        public async Task<IActionResult> ListarOrdens([FromBody] OrderPageRequest request)
        {
            _logger.LogInformation("Buscando lista de ordens paginadas. Página: {Page}, Tamanho: {PageSize}", request.Page, request.PageSize);

            try
            {
                var ordensPaginadas = await _repository.ListarOrdensPaginadasAsync(request);
                var ordensResponse = _mappingService.MapToListResponse(ordensPaginadas.Dados);

                var response = new OrderPageResponse<OrdemServicoListResponse>
                {
                    Dados = ordensResponse,
                    Count = ordensPaginadas.Count,
                    PageIndex = ordensPaginadas.PageIndex,
                    PageSize = ordensPaginadas.PageSize,
                    TotalPages = ordensPaginadas.TotalPages
                };

                _logger.LogInformation("Lista de ordens obtida com sucesso. Total: {Total}", ordensPaginadas.Count);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar lista de ordens");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ObterOrdemPorId(Guid id)
        {
            _logger.LogInformation("Buscando ordem por ID: {OrderId}", id);

            try
            {
                var ordem = await _repository.ObterOrdemPorIdAsync(id);
                if (ordem == null)
                {
                    _logger.LogWarning("Ordem não encontrada para ID: {OrderId}", id);
                    return NotFound("Ordem não encontrada");
                }

                var ordemResponse = _mappingService.MapToDetailsResponse(ordem);
                _logger.LogInformation("Ordem encontrada com sucesso para ID: {OrderId}", id);
                return Ok(ordemResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ordem por ID: {OrderId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
