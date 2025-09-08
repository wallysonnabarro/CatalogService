using System.Text;
using System.Text.Json;
using Web.Models;
using Web.Models.Resultados;

namespace Web.Services
{
    public class OrderService(IHttpClientFactory httpClientFactory, ICorrelationLogger correlationLogger) : IOrderService
    {
        public async Task<Result<Guid>> GerarOrdemServico(List<ProdutoParaOrdem> produtos)
        {
            correlationLogger.LogInformation("Gerando ordem de serviço para {Quantidade} produtos", produtos.Count);

            try
            {
                using var client = httpClientFactory.CreateClient("orderservices");
                var jsonContent = JsonSerializer.Serialize(produtos);
                var url = "/orders/orderns/gerar-ordem";

                var responseMessage = await client.PostAsync(url,
                    new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<Result<Guid>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    correlationLogger.LogInformation("Ordem de serviço gerada com sucesso. ID: {OrderId}", result?.Dados);
                    return result ?? Result<Guid>.Failed(new Errors { Description = "Resposta inválida" });
                }
                else
                {
                    correlationLogger.LogWarning("Falha ao gerar ordem de serviço. Status: {StatusCode}, Response: {Response}", 
                        responseMessage.StatusCode, responseContent);
                    return Result<Guid>.Failed(new Errors { Description = $"Erro HTTP {responseMessage.StatusCode}: {responseContent}" });
                }
            }
            catch (Exception e)
            {
                correlationLogger.LogError(e, "Erro ao gerar ordem de serviço.");
                return Result<Guid>.Failed(new Errors { Description = e.Message });
            }
        }
    }
}
