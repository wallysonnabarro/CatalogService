using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    public class LoginController(ILoginServices _login, IHttpContextAccessor _httpContext, IAntiforgery _antiforgery) : Controller
    {
        public async Task<IActionResult> Login(LoginModel login)
        {
            var resultado = await _login.Executar(login);

            if (resultado.Succeeded)
            {
                var tokensAntiForgery = _antiforgery.GetAndStoreTokens(_httpContext.HttpContext!);
                foreach (var cookie in resultado.Dados.Cookies)
                {
                    _httpContext.HttpContext!.Response.Cookies.Append(cookie.Name, cookie.Value, cookie.Options);
                }
            }

            return RedirectToAction("Index", "Dashboard");
        }
    }
}
