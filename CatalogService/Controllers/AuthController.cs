using CatalogService.Aplicacao;
using CatalogService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IJWTOAuth _jwt, IConfiguration _configuration) : ControllerBase
    {
        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Token([FromForm] TokenRequest request)
        {
            // Valida se as credenciais estão corretas
            if (!ValidarCredenciais(request.client_id, request.client_secret))
            {
                return Unauthorized(new { mensagem = "Credenciais inválidas" });
            }

            // Gera o token JWT
            var jwt = await _jwt.GetToken();

            if (jwt.Equals("Unauthorized"))
            {
                return Unauthorized(new { mensagem = "Erro ao gerar a autenticação com o Catálogo." });
            }

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
