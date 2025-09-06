using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using System.Text.Json;

namespace Web.Filter
{
    public class ValidarTokenFilter(IHttpClientFactory _httpClientFactory) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = context.HttpContext.Request.Cookies["Authorization"];

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            var valido = await IsAuthTokenValid(token.ToString().Replace("Bearer ", ""));

            if (valido == false)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            await next();
        }

        private async Task<bool> IsAuthTokenValid(string token)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient("webservices");

                var jsonConvenio = JsonSerializer.Serialize(token);
                var url = "api/auth/validate-token";

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
