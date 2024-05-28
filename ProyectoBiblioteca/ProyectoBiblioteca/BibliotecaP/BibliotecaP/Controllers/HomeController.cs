using Microsoft.AspNetCore.Mvc;
using BibliotecaP.Models;
using BibliotecaP.Models.dbModels;
using BibliotecaP.Models.DTOs;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace BibliotecaP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BibliotecaContext _context;

        public HomeController(ILogger<HomeController> logger, BibliotecaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["IsAuthenticated"] = User.Identity.IsAuthenticated;
            var rese�as = _context.Rese�as
                .Select(r => new Rese�aDTO
                {
                    IdRese�a = r.IdRese�a,
                    Descripcion = r.Descripcion,
                    Calificacion = r.Calificacion
                })
                .ToList();
            return View(rese�as);
        }

        [HttpPost]
        public IActionResult AddReview(string descripcion, int? calificacion)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var rese�a = new Rese�a
            {
                Descripcion = descripcion,
                UsuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), // Aseg�rate de que el ID del usuario est� disponible
                Calificacion = calificacion
            };
            _context.Rese�as.Add(rese�a);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Redirecciona a la vista donde se muestran las rese�as
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
