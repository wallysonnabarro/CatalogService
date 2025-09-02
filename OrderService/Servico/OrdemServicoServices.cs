using OrderService.Models;
using System.Text;
using System.Text.Json;

namespace OrderService.Servico
{
    public class OrdemServicoServices(IConfiguration _configuration) : IOrdemServicoServices
    {
        public async Task<List<ProdutosListaModels>> ValidarListaProdutos(List<Guid> lista)
        {
            using (HttpClient client = new HttpClient())
            {
                //Após testes, implementar a autenticação via token JWT

                // Converta o objeto para uma string JSON
                var jsonConvenio = JsonSerializer.Serialize(lista);

                var url = _configuration["apis:catalogo"] + "/api/produtos/validar-lista-produtos";

                var responseMessage = await client.PostAsync(url,
                                          new StringContent(jsonConvenio, Encoding.UTF8, "application/json"));

                using var contentStream = await responseMessage.Content.ReadAsStreamAsync();
                var retorno = await JsonSerializer.DeserializeAsync<List<ProdutosListaModels>>(contentStream);

                return retorno ?? new List<ProdutosListaModels>();
            }
        }
    }
}
