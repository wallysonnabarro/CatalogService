using Microsoft.AspNetCore.Mvc;
using OAuthServices.Aplicacao;
using OAuthServices.Models;
using OAuthServices.Services;

namespace OAuthServices.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IJWTOAuth _jwt, IConfiguration _configuration, ICorrelationLogger _logger) : ControllerBase
    {
        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Token([FromForm] TokenRequest request)
        {
            _logger.LogInformation("Iniciando processo de autenticação para client_id: {ClientId}", request.client_id);

            // Valida se as credenciais estão corretas
            if (!ValidarCredenciais(request.client_id, request.client_secret))
            {
                _logger.LogWarning("Tentativa de autenticação com credenciais inválidas para client_id: {ClientId}", request.client_id);
                return Unauthorized(new { mensagem = "Credenciais inválidas" });
            }

            _logger.LogInformation("Credenciais validadas com sucesso para client_id: {ClientId}", request.client_id);

            // Gera o token JWT
            var jwt = await _jwt.GetToken();

            if (jwt.Equals("Unauthorized"))
            {
                _logger.LogError("Erro ao gerar token JWT para client_id: {ClientId}", request.client_id);
                return Unauthorized(new { mensagem = "Erro ao gerar a autenticação com o Catálogo." });
            }

            _logger.LogInformation("Token JWT gerado com sucesso para client_id: {ClientId}", request.client_id);
            return Ok(new AccessToken { Access_token = jwt });
        }

        private bool ValidarCredenciais(string clientId, string clientSecret)
        {
            // Validação das credenciais
            var validClientId = _configuration["JwtSettings:ClienteId"];
            var validClientSecret = _configuration["JwtSettings:client_Secret"];

            return clientId == validClientId && clientSecret == validClientSecret;
        }
    }
}
