using Microsoft.AspNetCore.Mvc;

namespace BibliotecaP.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult IndexUsuario()
        {
            return View();
        }
    }
}
