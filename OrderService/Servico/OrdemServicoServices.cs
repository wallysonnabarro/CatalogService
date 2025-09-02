using OrderService.Models;
using System.Text;
using System.Text.Json;

namespace OrderService.Servico
{
    public class OrdemServicoServices(IConfiguration _configuration) : IOrdemServicoServices
    {
        public async Task AtualizarQuantidadeProdutos(List<Produto> lista)
        {
            using (HttpClient client = new HttpClient())
            {
                var jsonConvenio = JsonSerializer.Serialize(lista);
                var url = _configuration["apis:catalogo"] + "/api/produtos/atualizar-quantidade";

                var responseMessage = await client.PostAsync(url,
                                          new StringContent(jsonConvenio, Encoding.UTF8, "application/json"));
            }
        }

        public async Task<List<ProdutosListaModels>> ValidarListaProdutos(List<Guid> lista)
        {
            using (HttpClient client = new HttpClient())
            {
                var jsonConvenio = JsonSerializer.Serialize(lista);
                var url = _configuration["apis:catalogo"] + "/api/produtos/validar-lista-produtos";

                var responseMessage = await client.PostAsync(url,
                                          new StringContent(jsonConvenio, Encoding.UTF8, "application/json"));

                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                var retorno = JsonSerializer.Deserialize<List<ProdutosListaModels>>(responseContent);

                return retorno ?? new List<ProdutosListaModels>();
            }
        }
    }
}
