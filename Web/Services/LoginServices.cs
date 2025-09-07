using System.Text;
using System.Text.Json;
using Web.Models;
using Web.Models.Resultados;

namespace Web.Services
{
    public class LoginServices(IHttpClientFactory _httpClientFactory) : ILoginServices
    {
        public async Task<Result<AuthenticationResult>> Executar(LoginModel input)
        {
            try
            {
                if (input.Senha.Length < 10)
                {
                    return Result<AuthenticationResult>.Failed(new Errors() { Code = "205", Description = "A senha ser maior que 10" });
                }
                else if (!input.Senha.Any(char.IsPunctuation))
                {
                    return Result<AuthenticationResult>.Failed(new Errors() { Code = "205", Description = "A senha não contem um caracter especial" });
                }
                else if (input.Email.Equals(""))
                {
                    return Result<AuthenticationResult>.Failed(new Errors() { Code = "205", Description = "O usuário não foi informado" });
                }

                using var client = _httpClientFactory.CreateClient("webservices");

                var jsonConvenio = JsonSerializer.Serialize(input);
                var url = "/api/auth/autenticar";

                var responseMessage = await client.PostAsync(url,
                                              new StringContent(jsonConvenio, Encoding.UTF8, "application/json"));

                var responseContent = await responseMessage.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<AccessToken>(responseContent);

                // Extrair cookies da resposta
                var cookies = new List<CookieInfo>();

                if (responseMessage.Headers.TryGetValues("Set-Cookie", out var cookieHeaders))
                {
                    foreach (var cookieHeader in cookieHeaders)
                    {
                        var cookieParts = cookieHeader.Split(';');
                        var nameValue = cookieParts[0].Split('=');

                        if (nameValue.Length == 2)
                        {
                            cookies.Add(new CookieInfo
                            {
                                Name = nameValue[0].Trim(),
                                Value = nameValue[1].Trim(),
                                Options = new CookieOptions
                                {
                                    HttpOnly = cookieHeader.Contains("HttpOnly"),
                                    Secure = false,
                                    SameSite = cookieHeader.Contains("SameSite=Lax") ? SameSiteMode.Lax : SameSiteMode.None,
                                    Path = ExtractPath(cookieHeader),
                                    Expires = cookieHeader.Contains("Expires=") ?
                                              DateTimeOffset.Parse(cookieHeader.Split("Expires=")[1].Split(';')[0].Trim()) :
                                              (DateTimeOffset?)null
                                }
                            });
                        }
                    }
                }

                var result = new AuthenticationResult
                {
                    Token = token == null ? string.Empty: token.Access_token,
                    Cookies = cookies
                };

                return Result<AuthenticationResult>.Sucesso(result);
            }
            catch (Exception e)
            {
                return Result<AuthenticationResult>.Failed(new Errors { Description = e.Message });
            }
        }

        private string ExtractPath(string cookieHeader)
        {
            var pathMatch = System.Text.RegularExpressions.Regex.Match(cookieHeader, @"Path=([^;]+)");
            return pathMatch.Success ? pathMatch.Groups[1].Value : "/";
        }
    }
}
