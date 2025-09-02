using System.Text;
using System.Text.Json;

namespace OrderService.Servico
{
    public class OrdemServicoServices(IConfiguration _configuration) : IOrdemServicoServices
    {
        public async Task<List<Guid>> ValidarListaProdutos(List<Guid> lista)
        {
            using (HttpClient client = new HttpClient())
            {
                //Após testes, implementar a autenticação via token JWT

                // Converta o objeto para uma string JSON
                var jsonConvenio = JsonSerializer.Serialize(lista);

                var responseMessage = await client.PatchAsync(_configuration["apis:catalogo"] + "/api/produtos/validar-lista-produtos",
                                          new StringContent(jsonConvenio, Encoding.UTF8, "application/json"));

                using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

                var retorno = await JsonSerializer.DeserializeAsync<List<Guid>>(contentStream);

                return retorno ?? new List<Guid>();
            }
        }
    }
}
