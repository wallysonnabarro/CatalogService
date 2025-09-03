using OrderService.Models;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OrderService.Servico
{
    public class OrdemServicoServices(IConfiguration _configuration, IRabbitMqClient _rabbitMqClient) : IOrdemServicoServices
    {
        public async Task AtualizarQuantidadeProdutos(List<Produto> lista)
        {
            await _rabbitMqClient.PublicarAtualizarQuantidadeProdutosAsync(lista);


            //var token = await GerarToken();

            //using (HttpClient client = new HttpClient())
            //{
            //    client.DefaultRequestHeaders.Authorization =
            //        new AuthenticationHeaderValue("Bearer", token);

            //    var jsonConvenio = JsonSerializer.Serialize(lista);
            //    var url = _configuration["apis:catalogo"] + "/api/produtos/atualizar-quantidade";

            //    var responseMessage = await client.PostAsync(url,
            //                              new StringContent(jsonConvenio, Encoding.UTF8, "application/json"));
            //}
        }

        public async Task<List<ProdutosListaModels>> ValidarListaProdutos(List<Guid> lista)
        {
            //var token = await GerarToken();

            using (HttpClient client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Authorization =
                //    new AuthenticationHeaderValue("Bearer", token);

                var jsonConvenio = JsonSerializer.Serialize(lista);
                var url = _configuration["apis:catalogo"] + "/catalog/produtos/validar-lista-produtos";

                var responseMessage = await client.PostAsync(url,
                                          new StringContent(jsonConvenio, Encoding.UTF8, "application/json"));

                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                var retorno = JsonSerializer.Deserialize<List<ProdutosListaModels>>(responseContent);

                return retorno ?? new List<ProdutosListaModels>();
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
