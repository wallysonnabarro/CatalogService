using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
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
                        _httpContext.HttpContext!.Response.Cookies.Append(cookie.Name, cookie.Value, cookie.Options);
                    }
                    
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
