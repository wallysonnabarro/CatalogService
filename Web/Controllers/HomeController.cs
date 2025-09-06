using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsuarioServices _usuarioServices;

        public HomeController(ILogger<HomeController> logger, IUsuarioServices usuarioServices)
        {
            _logger = logger;
            _usuarioServices = usuarioServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email)
        {
            var emailExiste = await _usuarioServices.VerificarEmailExiste(email);

            if (emailExiste)
            {
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

            TempData["resultMessage"] = "Sucesso!";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
