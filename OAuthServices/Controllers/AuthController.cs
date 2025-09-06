using Microsoft.AspNetCore.Mvc;
using OAuthServices.Aplicacao;
using OAuthServices.Data;
using OAuthServices.Models;
using OAuthServices.Services;
using System.Threading.Tasks;

namespace OAuthServices.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IJWTOAuth _jwt, IConfiguration _configuration, ICorrelationLogger _logger,
        IAuthenticationRepository _authentication, IRoleRepository _roleRepository, ITokenService _tokens,
        ITokenRepository _tokenrepository) : ControllerBase
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

        [Route("autenticar")]
        [HttpPost]
        public async Task<IActionResult> Autenticar(LoginModel model)
        {
            var resultado = await _authentication.AuthenticateEmail(model);

            if (!resultado.Succeeded) return Unauthorized(resultado.ToString());

            var roles = await _roleRepository.GetRolesByUserId(resultado.Dados);

            var access = await _tokens.CreateAccessToken(resultado.Dados, roles, TimeSpan.FromMinutes(5));
            var (refreshToken, jti, exp) = await _tokens.CreateRefreshToken(resultado.Dados);

            // cookie com jti + segredo; aqui simples: guarde jti e um valor random
            Response.Cookies.Append("refresh_jti", jti, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = exp,
                Path = "bff/auth/refresh"
            });
            Response.Cookies.Append("refresh_val", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = exp,
                Path = "bff/auth/refresh"
            });

            // (opcional) XSRF para POST/PUT/DELETE
            Response.Cookies.Append("XSRF-TOKEN", Guid.NewGuid().ToString("N"),
                new CookieOptions { HttpOnly = false, Secure = true, SameSite = SameSiteMode.Lax });

            return Ok(new { accessToken = access });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var jti = Request.Cookies["refresh_jti"];
            var val = Request.Cookies["refresh_val"];
            if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(val)) return Unauthorized();

            // valide integridade do refresh (ex.: comparar com DB/hash)
            var refresh = await _tokens.TryUseRefresh(Guid.Parse(jti));

            if (refresh.existe == false) return Unauthorized();
            if (refresh.userId == null) return Unauthorized();

            var roles = await _roleRepository.GetRolesByUserId((Guid)refresh.userId);
            var access = _tokens.CreateAccessToken(refresh.userId, roles, TimeSpan.FromMinutes(5));
            var (newVal, newJti, exp) = await _tokens.CreateRefreshToken(refresh.userId);

            Response.Cookies.Append("refresh_jti", newJti, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = exp,
                Path = "bff/auth/refresh"
            });
            Response.Cookies.Append("refresh_val", newVal, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = exp,
                Path = "bff/auth/refresh"
            });

            return Ok(new { accessToken = access });
        }

        [HttpPost("revoke")]
        public IActionResult Revoke()
        {
            var jti = Request.Cookies["refresh_jti"];
            if (!string.IsNullOrEmpty(jti)) _tokenrepository.Remove(Guid.Parse(jti));

            // apaga cookies
            Response.Cookies.Delete("refresh_jti", new CookieOptions { Path = "bff/auth/revoke" });
            Response.Cookies.Delete("refresh_val", new CookieOptions { Path = "bff/auth/revoke" });

            return NoContent();
        }

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken(string token)
        {
            var valido = await _tokens.ValidarToken(token);

            if(valido == false) return Unauthorized(valido);

            return Ok(valido);
        }
    }
}
