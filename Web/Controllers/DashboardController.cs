using Microsoft.AspNetCore.Mvc;
using Web.Filter;

namespace Web.Controllers
{
    [ServiceFilter(typeof(ValidarTokenFilter))]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Sair()
        {
            Response.Cookies.Delete("Authorization");
            Response.Cookies.Delete("UserEmail");
            Response.Cookies.Delete("XSRF-TOKEN");
            return RedirectToAction("Index", "Home");
        }
    }
}
