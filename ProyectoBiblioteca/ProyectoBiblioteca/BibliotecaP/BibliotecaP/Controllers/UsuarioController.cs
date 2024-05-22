using BibliotecaP.Models.dbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaP.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly BibliotecaContext _context;

        public UsuarioController(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> IndexUsuario()
        {
            var userId = 1; // Reemplaza esto con la lógica para obtener el ID del usuario autenticado
            var usuario = await _context.Users.Include(u => u.ReservacionCubiculos).FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null)
            {
                return NotFound();
            }

            var reservaActiva = usuario.ReservacionCubiculos.OrderByDescending(r => r.FechaHoraInicio).FirstOrDefault();
            ViewBag.ReservaActiva = reservaActiva;

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> ReservaInfo(int reservaId)
        {
            var reservacionCubiculo = await _context.ReservacionCubiculos
                .Include(r => r.Cubiculo)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.ReservacionId == reservaId);

            if (reservacionCubiculo == null)
            {
                return NotFound();
            }

            var accesoQrBytes = QrCodeGenerator.GenerateQrCode($"Acceso: {reservacionCubiculo.ReservacionId}");
            var salidaQrBytes = QrCodeGenerator.GenerateQrCode($"Salida: {reservacionCubiculo.ReservacionId}");

            ViewBag.AccesoQrBase64 = Convert.ToBase64String(accesoQrBytes);
            ViewBag.SalidaQrBase64 = Convert.ToBase64String(salidaQrBytes);

            return View(reservacionCubiculo);
        }

    }
}
