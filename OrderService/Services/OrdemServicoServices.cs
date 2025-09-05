using OrderService.Models;
using System.Text;
using System.Text.Json;

namespace OrderService.Services
{
    public class OrdemServicoServices(IConfiguration _configuration, IRabbitMqClient _rabbitMqClient, 
        ICorrelationLogger _logger, IHttpClientFactory _httpClientFactory) : IOrdemServicoServices
    {
        public async Task AtualizarQuantidadeProdutos(List<Produto> lista)
        {
            _logger.LogInformation("Iniciando a atualização da {QuantidadeProdutos} produtos - evento", lista.Count);

            await _rabbitMqClient.PublicarAtualizarQuantidadeProdutosAsync(lista);
        }

        public async Task<List<ProdutosListaModels>> ValidarListaProdutos(List<Guid> lista)
        {
            _logger.LogInformation("Iniciando a validação da lista de produtos - https catalog", lista.Count);

            try
            {
                using var client = _httpClientFactory.CreateClient("CatalogService");

                var jsonConvenio = JsonSerializer.Serialize(lista);
                var url = "/catalog/produtos/validar-lista-produtos";

                var responseMessage = await client.PostAsync(url,
                                              new StringContent(jsonConvenio, Encoding.UTF8, "application/json"));

                var responseContent = await responseMessage.Content.ReadAsStringAsync();
                var retorno = JsonSerializer.Deserialize<List<ProdutosListaModels>>(responseContent);

                return retorno ?? new List<ProdutosListaModels>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro validar lista de produtos - https catalog", lista.Count);
                throw;
            }
        }

        private async Task<string> GerarToken()
        {
            using (HttpClient client = new HttpClient())
            {
                var tokenRequest = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", _configuration["JwtSettings:ClienteId"]!),
                    new KeyValuePair<string, string>("client_secret", _configuration["JwtSettings:client_Secret"]!)
                });

                var url = _configuration["apis:catalogo"] + "/api/auth/token";
                var tokenResponse = await client.PostAsync(url, tokenRequest);
                var tokenResult = await JsonSerializer.DeserializeAsync<AccessToken>(await tokenResponse.Content.ReadAsStreamAsync());

                return tokenResult == null ? string.Empty : tokenResult.Access_token;
            }
        }
    }
}
