using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using BibliotecaP.Models.dbModels;

namespace BibliotecaP.Services
{
    public class ReservationService
    {
        private readonly BibliotecaContext _context;
        private readonly IHubContext<CubiculoHub> _hubContext;

        public ReservationService(BibliotecaContext context, IHubContext<CubiculoHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<int> CreateReservation(int userId, int cubiculoId, DateTime startDate, DateTime endDate)
        {
            var reservation = new ReservacionCubiculo
            {
                UsuarioId = userId,
                CubiculoId = cubiculoId,
                FechaHoraInicio = startDate,
                FechaHoraFin = endDate
            };

            _context.ReservacionCubiculos.Add(reservation);

            var cubiculo = await _context.Cubiculos.FindAsync(cubiculoId);
            if (cubiculo != null)
            {
                cubiculo.EstadoId = 2; // Asegúrate que el 2 corresponde a 'Ocupado'
                _context.Cubiculos.Update(cubiculo);
            }

            await _context.SaveChangesAsync();

            // Notificar a todos los clientes sobre el cambio de estado
            await _hubContext.Clients.All.SendAsync("CubiculoEstadoActualizado", cubiculoId, "Ocupado");

            // Programar la tarea para volver a cambiar el estado del cubículo a 'Disponible'
            var delay = endDate - DateTime.UtcNow;
            if (delay.TotalMilliseconds > 0)
            {
                _ = Task.Delay(delay).ContinueWith(async _ =>
                {
                    await CambiarEstadoCubiculo(cubiculoId, 1); // Asumiendo que 1 es el estado 'Disponible'
                });
            }

            return reservation.ReservacionId;
        }

        public async Task<List<Cubiculo>> GetCubiculos()
        {
            return await _context.Cubiculos.Include(c => c.Estado).ToListAsync();
        }

        public async Task<List<ReservacionCubiculo>> GetUserReservations(int userId)
        {
            return await _context.ReservacionCubiculos
                .Include(r => r.Cubiculo)
                .Where(r => r.UsuarioId == userId)
                .ToListAsync();
        }

        private async Task CambiarEstadoCubiculo(int cubiculoId, int nuevoEstadoId)
        {
            var cubiculo = await _context.Cubiculos.FindAsync(cubiculoId);
            if (cubiculo != null)
            {
                cubiculo.EstadoId = nuevoEstadoId;
                _context.Cubiculos.Update(cubiculo);
                await _context.SaveChangesAsync();

                // Notificar a los clientes sobre el cambio de estado
                var estado = nuevoEstadoId == 1 ? "Disponible" : "Ocupado"; // Ajustar según los estados disponibles
                await _hubContext.Clients.All.SendAsync("CubiculoEstadoActualizado", cubiculoId, estado);
            }
        }
    }
}
