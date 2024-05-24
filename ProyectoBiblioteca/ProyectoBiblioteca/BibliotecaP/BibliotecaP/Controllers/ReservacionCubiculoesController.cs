using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BibliotecaP.Models.dbModels;
using BibliotecaP.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaP.Controllers
{
    public class ReservacionCubiculoesController : Controller
    {
        private readonly BibliotecaContext _context;

        // Constructor: Inicializa una nueva instancia del ReservacionCubiculoesController
        // y asigna el contexto de la base de datos a una variable privada.
        public ReservacionCubiculoesController(BibliotecaContext context)
        {
            _context = context;
        }

        // Método Index: Recupera y muestra una lista de reservas de cubículos.
        // Obtiene todas las reservas de cubículos junto con la información del cubículo y el usuario asociado y devolverla a la vista.
        public async Task<IActionResult> Index()
        {
            var bibliotecaContext = _context.ReservacionCubiculos.Include(r => r.Cubiculo).Include(r => r.Usuario);
            return View(await bibliotecaContext.ToListAsync());
        }

        // Método Details: Muestra los detalles de una reserva de cubículo específica.
        // Obtiene una reserva de cubículo por su ID y devolverla a la vista de detalles.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservacionCubiculo = await _context.ReservacionCubiculos
                .Include(r => r.Cubiculo)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.ReservacionId == id);
            if (reservacionCubiculo == null)
            {
                return NotFound();
            }

            return View(reservacionCubiculo);
        }

        // Método Create (GET): Muestra el formulario para crear una nueva reserva de cubículo.
        // Prepara y devuelve la vista con listas de selección para cubículos y usuarios.
        public IActionResult Create()
        {
            ViewData["CubiculoId"] = new SelectList(_context.Cubiculos, "CubiculoId", "CubiculoId");
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // Método Create (POST): Maneja la creación de una nueva reserva de cubículo.
        // Valida y guarda una nueva reserva en la base de datos y redirigir al índice de reservas.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservacionId,CubiculoId,UsuarioId,FechaHoraInicio,FechaHoraFin")] ReservacionCubiculo reservacionCubiculo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservacionCubiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CubiculoId"] = new SelectList(_context.Cubiculos, "CubiculoId", "CubiculoId", reservacionCubiculo.CubiculoId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", reservacionCubiculo.UsuarioId);
            return View(reservacionCubiculo);
        }

        // Método Edit (GET): Muestra el formulario para editar una reserva de cubículo existente.
        // Prepara y devuelve la vista con los datos de la reserva a editar.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservacionCubiculo = await _context.ReservacionCubiculos.FindAsync(id);
            if (reservacionCubiculo == null)
            {
                return NotFound();
            }
            ViewData["CubiculoId"] = new SelectList(_context.Cubiculos, "CubiculoId", "CubiculoId", reservacionCubiculo.CubiculoId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", reservacionCubiculo.UsuarioId);
            return View(reservacionCubiculo);
        }

        // Método Edit (POST): Maneja la actualización de una reserva de cubículo existente.
        // Valida y guarda los cambios en la base de datos y redirigir al índice de reservas.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservacionId,CubiculoId,UsuarioId,FechaHoraInicio,FechaHoraFin")] ReservacionCubiculo reservacionCubiculo)
        {
            if (id != reservacionCubiculo.ReservacionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservacionCubiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservacionCubiculoExists(reservacionCubiculo.ReservacionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CubiculoId"] = new SelectList(_context.Cubiculos, "CubiculoId", "CubiculoId", reservacionCubiculo.CubiculoId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", reservacionCubiculo.UsuarioId);
            return View(reservacionCubiculo);
        }

        // Método Delete (GET): Muestra la vista de confirmación para eliminar una reserva de cubículo.
        // Obtiene una reserva de cubículo por su ID y devolverla a la vista de eliminación.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservacionCubiculo = await _context.ReservacionCubiculos
                .Include(r => r.Cubiculo)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.ReservacionId == id);
            if (reservacionCubiculo == null)
            {
                return NotFound();
            }

            return View(reservacionCubiculo);
        }

        // Método Delete (POST): Maneja la confirmación de eliminación de una reserva de cubículo.
        // Elimina la reserva de cubículo de la base de datos y redirige al índice de reservas.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservacionCubiculo = await _context.ReservacionCubiculos.FindAsync(id);
            if (reservacionCubiculo != null)
            {
                _context.ReservacionCubiculos.Remove(reservacionCubiculo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Método ReservacionCubiculoExists: Verifica si una reserva de cubículo existe en la base de datos.
        // Comprueba la existencia de una reserva antes de intentar actualizarla o eliminarla.
        private bool ReservacionCubiculoExists(int id)
        {
            return _context.ReservacionCubiculos.Any(e => e.ReservacionId == id);
        }
    }
}
