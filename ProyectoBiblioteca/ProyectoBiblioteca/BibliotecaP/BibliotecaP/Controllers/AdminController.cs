using Microsoft.AspNetCore.Mvc;

namespace BibliotecaP.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
