using System.Text;
using System.Text.Json;
using Web.Models;
using Web.Models.Resultados;

namespace Web.Services
{
    public class OrderService(IHttpClientFactory httpClientFactory, ICorrelationLogger correlationLogger) : IOrderService
    {
        public async Task<Result<OrderGeradaModel>> GerarOrdemServico(List<ProdutoParaOrdem> produtos)
        {
            correlationLogger.LogInformation("Gerando ordem de serviço para {Quantidade} produtos", produtos.Count);

            try
            {
                using var client = httpClientFactory.CreateClient("gatewayservices");
                var jsonContent = JsonSerializer.Serialize(produtos);
                var url = "/orders/orderns/gerar-ordem";

                var responseMessage = await client.PostAsync(url,
                    new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<OrderGeradaModel>(responseContent);

                    if (result == null)
                    {
                        correlationLogger.LogError($"a requisição para {url}, retornou nulo.", "Resposta inválida.");
                        return Result<OrderGeradaModel>.Failed(new Errors { Description = "Resposta inválida" });
                    }

                    correlationLogger.LogInformation($"Ordem de serviço gerada com sucesso. ID: {result.Id}");
                    return Result<OrderGeradaModel>.Sucesso(result);
                }
                else
                {
                    correlationLogger.LogWarning("Falha ao gerar ordem de serviço. Status: {StatusCode}, Response: {Response}",
                        responseMessage.StatusCode, responseContent);
                    return Result<OrderGeradaModel>.Failed(new Errors { Description = $"Erro HTTP {responseMessage.StatusCode}: {responseContent}" });
                }
            }
            catch (Exception e)
            {
                correlationLogger.LogError(e, "Erro ao gerar ordem de serviço.");
                return Result<OrderGeradaModel>.Failed(new Errors { Description = e.Message });
            }
        }

        public async Task<OrdensPaginadasViewModel> ListarOrdensPaginadasAsync(PageWrapper pageWrapper)
        {
            correlationLogger.LogInformation("Iniciando busca de ordens paginadas. Página: {Page}, Tamanho: {PageSize}", 
                pageWrapper.Page, pageWrapper.PageSize);

            try
            {
                using var client = httpClientFactory.CreateClient("gatewayservices");

                var jsonContent = JsonSerializer.Serialize(pageWrapper);
                var url = "/orders/orderns/listar-ordens";

                correlationLogger.LogInformation("Enviando requisição para buscar ordens paginadas. URL: {Url}", url);

                var responseMessage = await client.PostAsync(url,
                    new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    var paginacaoResponse = JsonSerializer.Deserialize<PaginacaoResponse<OrdemServicoListViewModel>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var ordens = paginacaoResponse?.Dados ?? new List<OrdemServicoListViewModel>();
                    
                    var viewModel = new OrdensPaginadasViewModel
                    {
                        Ordens = ordens,
                        PaginaAtual = pageWrapper.Page,
                        TamanhoPagina = pageWrapper.PageSize,
                        TotalItens = paginacaoResponse?.Count ?? 0
                    };

                    correlationLogger.LogInformation("Ordens paginadas obtidas com sucesso. Total: {Total}", viewModel.TotalItens);
                    return viewModel;
                }
                else
                {
                    correlationLogger.LogWarning("Falha ao obter ordens paginadas. Status: {StatusCode}, Response: {Response}", 
                        responseMessage.StatusCode, responseContent);
                    
                    return new OrdensPaginadasViewModel
                    {
                        Ordens = new List<OrdemServicoListViewModel>(),
                        PaginaAtual = pageWrapper.Page,
                        TamanhoPagina = pageWrapper.PageSize,
                        TotalItens = 0
                    };
                }
            }
            catch (Exception e)
            {
                correlationLogger.LogError(e, "Erro ao obter ordens paginadas");
                throw new Exception("Erro ao buscar ordens: " + e.Message);
            }
        }

        public async Task<OrdemDetalhesViewModel?> ObterOrdemPorIdAsync(Guid id)
        {
            correlationLogger.LogInformation("Iniciando busca de ordem por ID: {Id}", id);

            try
            {
                using var client = httpClientFactory.CreateClient("gatewayservices");

                var url = $"/orders/orderns/{id}";

                correlationLogger.LogInformation("Enviando requisição para buscar ordem por ID. URL: {Url}", url);

                var responseMessage = await client.GetAsync(url);
                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    var ordem = JsonSerializer.Deserialize<OrdemDetalhesViewModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    correlationLogger.LogInformation("Ordem encontrada com sucesso para ID: {Id}", id);
                    return ordem;
                }
                else
                {
                    correlationLogger.LogWarning("Ordem não encontrada para ID: {Id}. Status: {StatusCode}, Response: {Response}", 
                        id, responseMessage.StatusCode, responseContent);
                    return null;
                }
            }
            catch (Exception e)
            {
                correlationLogger.LogError(e, "Erro ao buscar ordem por ID: {Id}", id);
                throw new Exception("Erro ao buscar ordem: " + e.Message);
            }
        }
    }
}
