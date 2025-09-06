using System.Text;
using System.Text.Json;
using Web.Models;

namespace Web.Services
{
    public class UsuarioServices(IHttpClientFactory _httpClientFactory) : IUsuarioServices
    {
        public async Task NovoUsuario(NovoUsuarioModel model)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient("webservices");

                var jsonConvenio = JsonSerializer.Serialize(model);
                var url = "/api/usuario/novo";

                var responseMessage = await client.PostAsync(url,
                                              new StringContent(jsonConvenio, Encoding.UTF8, "application/json"));

                await responseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> VerificarEmailExiste(string email)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient("webservices");

                var jsonConvenio = JsonSerializer.Serialize(email);
                var url = "/api/usuario/confirmar-email";

                var responseMessage = await client.PostAsync(url,
                                              new StringContent(jsonConvenio, Encoding.UTF8, "application/json"));

                var responseContent = await responseMessage.Content.ReadAsStringAsync();
                bool existe = JsonSerializer.Deserialize<bool>(responseContent);

                return existe;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
