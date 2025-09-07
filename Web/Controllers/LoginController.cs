using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    public class LoginController(ILoginServices _login, IHttpContextAccessor _httpContext, IAntiforgery _antiforgery, ICorrelationLogger _correlationLogger) : Controller
    {
        public async Task<IActionResult> Login(LoginModel login)
        {
            _correlationLogger.LogInformation("Tentativa de login iniciada para o email: {Email}", login.Email);

            try
            {
                var resultado = await _login.Executar(login);

                if (resultado.Succeeded)
                {
                    _correlationLogger.LogInformation("Login realizado com sucesso para o email: {Email}", login.Email);

                    var tokensAntiForgery = _antiforgery.GetAndStoreTokens(_httpContext.HttpContext!);
                    foreach (var cookie in resultado.Dados.Cookies)
                    {
                        if (cookie.Name.Equals("refresh_jti"))
                            Response.Cookies.Append("refresh_jti", cookie.Value, new CookieOptions
                            {
                                HttpOnly = cookie.Options.HttpOnly,
                                Secure = cookie.Options.Secure,
                                SameSite = cookie.Options.SameSite,
                                Expires = cookie.Options.Expires,
                                Path = cookie.Options.Path
                            });

                        if (cookie.Name.Equals("refresh_val"))
                            Response.Cookies.Append("refresh_val", cookie.Value, new CookieOptions
                            {
                                HttpOnly = cookie.Options.HttpOnly,
                                Secure = cookie.Options.Secure,
                                SameSite = cookie.Options.SameSite,
                                Expires = cookie.Options.Expires,
                                Path = cookie.Options.Path
                            });

                    }

                    var expiration = login.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)  // ← 30 dias se marcar "lembrar-me"
                    : DateTimeOffset.UtcNow.AddHours(1);

                    Response.Cookies.Append("Authorization", $"Bearer {resultado.Dados.Token}", new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = expiration
                    });

                    // (opcional) XSRF para POST/PUT/DELETE
                    Response.Cookies.Append("XSRF-TOKEN", Guid.NewGuid().ToString("N"),
                        new CookieOptions { HttpOnly = false, Secure = true, SameSite = SameSiteMode.Lax });

                    _correlationLogger.LogInformation("Cookies de autenticação configurados com sucesso para o email: {Email}", login.Email);
                }
                else
                {
                    _correlationLogger.LogWarning("Falha na autenticação para o email: {Email}. Motivo: {Error}",
                        login.Email, resultado.ToString() ?? "Credenciais inválidas");
                }

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                _correlationLogger.LogError(ex, "Erro durante o processo de login para o email: {Email}", login.Email);

                return RedirectToAction("Index", "Home", new { resultMessage = "Erro interno do servidor. Tente novamente." });
            }
        }
    }
}
