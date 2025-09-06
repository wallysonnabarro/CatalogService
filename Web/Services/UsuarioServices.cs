using System.Text;
using System.Text.Json;
using Web.Models;

namespace Web.Services
{
    public class UsuarioServices(IHttpClientFactory _httpClientFactory, ICorrelationLogger _correlationLogger) : IUsuarioServices
    {
        public async Task NovoUsuario(NovoUsuarioModel model)
        {
            _correlationLogger.LogInformation("Iniciando criação de novo usuário para o email: {Email}", model.Email);

            try
            {
                using var client = _httpClientFactory.CreateClient("webservices");

                var jsonConvenio = JsonSerializer.Serialize(model);
                var url = "/api/usuario/novo";

                _correlationLogger.LogInformation("Enviando requisição para criar usuário. URL: {Url}, Email: {Email}", url, model.Email);

                var responseMessage = await client.PostAsync(url,
                                              new StringContent(jsonConvenio, Encoding.UTF8, "application/json"));

                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    _correlationLogger.LogInformation("Usuário criado com sucesso para o email: {Email}. Status: {StatusCode}", 
                        model.Email, responseMessage.StatusCode);
                }
                else
                {
                    _correlationLogger.LogWarning("Falha ao criar usuário para o email: {Email}. Status: {StatusCode}, Response: {Response}", 
                        model.Email, responseMessage.StatusCode, responseContent);
                }
            }
            catch (Exception e)
            {
                _correlationLogger.LogError(e, "Erro ao criar usuário para o email: {Email}", model.Email);
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> VerificarEmailExiste(string email)
        {
            _correlationLogger.LogInformation("Iniciando verificação de existência do email: {Email}", email);

            try
            {
                using var client = _httpClientFactory.CreateClient("webservices");

                var jsonConvenio = JsonSerializer.Serialize(email);
                var url = "/api/usuario/confirmar-email";

                _correlationLogger.LogInformation("Enviando requisição para verificar email. URL: {Url}, Email: {Email}", url, email);

                var responseMessage = await client.PostAsync(url,
                                              new StringContent(jsonConvenio, Encoding.UTF8, "application/json"));

                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    bool existe = JsonSerializer.Deserialize<bool>(responseContent);
                    
                    _correlationLogger.LogInformation("Verificação de email concluída para: {Email}. Existe: {Existe}, Status: {StatusCode}", 
                        email, existe, responseMessage.StatusCode);
                    
                    return existe;
                }
                else
                {
                    _correlationLogger.LogWarning("Falha ao verificar email: {Email}. Status: {StatusCode}, Response: {Response}", 
                        email, responseMessage.StatusCode, responseContent);
                    
                    // Em caso de falha na API, assumir que o email não existe para permitir criação
                    return false;
                }
            }
            catch (Exception e)
            {
                _correlationLogger.LogError(e, "Erro ao verificar existência do email: {Email}", email);
                
                // Em caso de erro, assumir que o email não existe para permitir criação
                return false;
            }
        }
    }
}
