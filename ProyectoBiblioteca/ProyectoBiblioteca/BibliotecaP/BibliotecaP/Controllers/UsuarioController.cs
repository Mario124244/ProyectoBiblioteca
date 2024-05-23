using BibliotecaP.Models;
using BibliotecaP.Models.dbModels;
using BibliotecaP.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

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

    reservacionCubiculo.AccesoQrBase64 = Convert.ToBase64String(accesoQrBytes);
    reservacionCubiculo.SalidaQrBase64 = Convert.ToBase64String(salidaQrBytes);

    _context.Update(reservacionCubiculo);
    await _context.SaveChangesAsync();

    ViewBag.AccesoQrBase64 = reservacionCubiculo.AccesoQrBase64;
    ViewBag.SalidaQrBase64 = reservacionCubiculo.SalidaQrBase64;

    return View(reservacionCubiculo);
}

  public async Task<IActionResult> HistorialReservas()
        {
            var userId = 1; // Reemplaza esto con la lógica para obtener el ID del usuario autenticado
            var usuario = await _context.Users
                .Include(u => u.ReservacionCubiculos)
                .ThenInclude(r => r.Cubiculo)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null)
            {
                return NotFound();
            }

            // Especifica la zona horaria deseada (ejemplo: "Central Standard Time" para CST)
            var timeZoneId = "Etc/GMT+6";
            var timeZoneInfo = TZConvert.GetTimeZoneInfo(timeZoneId);

            var reservas = usuario.ReservacionCubiculos
                .OrderByDescending(r => r.FechaHoraInicio) // Ordenar por FechaHoraInicio en orden descendente
                .Select(r => new ReservaViewModel
                {
                    Id = r.ReservacionId,
                    Nombre = r.Cubiculo?.Nombre ?? "Cubículo no disponible",
                    HoraEntrada = TimeZoneInfo.ConvertTimeFromUtc(r.FechaHoraInicio, timeZoneInfo).ToString("HH:mm"), // Convierte a la zona horaria especificada y formatea
                    HoraSalida = TimeZoneInfo.ConvertTimeFromUtc(r.FechaHoraFin, timeZoneInfo).ToString("HH:mm"),     // Convierte a la zona horaria especificada y formatea
                    AccesoQrBase64 = r.AccesoQrBase64 ?? string.Empty,
                    SalidaQrBase64 = r.SalidaQrBase64 ?? string.Empty
                }).ToList();

            return View(reservas);
        }
 int Id { get; set; }
        public string Nombre { get; set; }
        public string HoraEntrada { get; set; }
        public string HoraSalida { get; set; }
        public string AccesoQrBase64 { get; set; }
        public string SalidaQrBase64 { get; set; }
    }
}
