using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsuarioServices _usuarioServices;
        private readonly ICorrelationLogger _correlationLogger;

        public HomeController(ILogger<HomeController> logger, IUsuarioServices usuarioServices, ICorrelationLogger correlationLogger)
        {
            _logger = logger;
            _usuarioServices = usuarioServices;
            _correlationLogger = correlationLogger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email)
        {
            _correlationLogger.LogInformation("Tentativa de criação de usuário de teste para o email: {Email}", email);

            try
            {
                var emailExiste = await _usuarioServices.VerificarEmailExiste(email);

                if (emailExiste)
                {
                    _correlationLogger.LogWarning("Tentativa de criar usuário de teste com email já existente: {Email}", email);
                    TempData["resultMessage"] = "E-mail já cadastrado.";
                    return View();
                }

                var novo = new NovoUsuarioModel
                {
                    Nome = "docker compose",
                    Email = email,
                    Senha = "123456dC@d1",
                    ConfirmarSenha = "123456dC@d1"
                };

                await _usuarioServices.NovoUsuario(novo);

                _correlationLogger.LogInformation("Usuário de teste criado com sucesso para o email: {Email}", email);
                TempData["resultMessage"] = "Sucesso!";
                return View();
            }
            catch (Exception ex)
            {
                _correlationLogger.LogError(ex, "Erro ao criar usuário de teste para o email: {Email}", email);
                TempData["resultMessage"] = "Erro ao criar usuário. Tente novamente.";
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier,
                ErrorMessage = TempData["ErrorMessage"]?.ToString(),
                ErrorCode = TempData["ErrorCode"]?.ToString(),
                CorrelationId = HttpContext.Items["CorrelationId"]?.ToString()
            };

            return View(errorViewModel);
        }
    }
}