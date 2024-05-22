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

        public ReservacionCubiculoesController(BibliotecaContext context)
        {
            _context = context;
        }

        // GET: ReservacionCubiculoes
        public async Task<IActionResult> Index()
        {
            var bibliotecaContext = _context.ReservacionCubiculos.Include(r => r.Cubiculo).Include(r => r.Usuario);
            return View(await bibliotecaContext.ToListAsync());
        }

        // GET: ReservacionCubiculoes/Details/5
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

        // GET: ReservacionCubiculoes/Create
        public IActionResult Create()
        {
            ViewData["CubiculoId"] = new SelectList(_context.Cubiculos, "CubiculoId", "CubiculoId");
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ReservacionCubiculoes/Create
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

        // Nueva acción para mostrar la información de la reserva
       

        // GET: ReservacionCubiculoes/Edit/5
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

        // POST: ReservacionCubiculoes/Edit/5
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

        // GET: ReservacionCubiculoes/Delete/5
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

        // POST: ReservacionCubiculoes/Delete/5
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

        private bool ReservacionCubiculoExists(int id)
        {
            return _context.ReservacionCubiculos.Any(e => e.ReservacionId == id);
        }
    }
}
