using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using BibliotecaP.Models.dbModels;
using BibliotecaP.Models;

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

        public async Task<IEnumerable<ReservacionCubiculo>> GetUserReservations(int userId)
        {
            return await _context.ReservacionCubiculos
                .Where(r => r.UsuarioId == userId)
                .OrderByDescending(r => r.FechaHoraInicio)
                .ToListAsync();
        }

        public async Task<int> CreateCubiculoReservation(int userId, int cubiculoId, DateTime startDate, DateTime endDate)
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

        public async Task<int> CreateMesaReservation(int userId, int mesaId, DateTime startDate, DateTime endDate)
        {
            var reservation = new ReservacionMesa
            {
                UsuarioId = userId,
                MesaId = mesaId,
                FechaHoraInicio = startDate,
                FechaHoraFin = endDate
            };

            _context.ReservacionMesas.Add(reservation);

            var mesa = await _context.Mesas.FindAsync(mesaId);
            if (mesa != null)
            {
                mesa.EstadoId = 2; // Asegúrate que el 2 corresponde a 'Ocupado'
                _context.Mesas.Update(mesa);
            }

            await _context.SaveChangesAsync();

            // Notificar a todos los clientes sobre el cambio de estado
            await _hubContext.Clients.All.SendAsync("MesaEstadoActualizado", mesaId, "Ocupado");

            // Programar la tarea para volver a cambiar el estado de la mesa a 'Disponible'
            var delay = endDate - DateTime.UtcNow;
            if (delay.TotalMilliseconds > 0)
            {
                _ = Task.Delay(delay).ContinueWith(async _ =>
                {
                    await CambiarEstadoMesa(mesaId, 1); // Asumiendo que 1 es el estado 'Disponible'
                });
            }

            return reservation.ReservacionId;
        }

        public async Task<List<Cubiculo>> GetCubiculos()
        {
            return await _context.Cubiculos.Include(c => c.Estado).ToListAsync();
        }

        public async Task<List<Mesa>> GetMesas()
        {
            return await _context.Mesas.Include(m => m.Estado).ToListAsync();
        }

        public async Task<List<ReservacionCubiculo>> GetUserCubiculoReservations(int userId)
        {
            return await _context.ReservacionCubiculos
                .Include(r => r.Cubiculo)
                .Where(r => r.UsuarioId == userId)
                .ToListAsync();
        }

        public async Task<List<ReservacionMesa>> GetUserMesaReservations(int userId)
        {
            return await _context.ReservacionMesas
                .Include(r => r.Mesa)
                .Where(r => r.UsuarioId == userId)
                .ToListAsync();
        }

        public async Task<bool> UpdateCubiculoEstado(int cubiculoId, string estado)
        {
            var cubiculo = await _context.Cubiculos.FindAsync(cubiculoId);
            if (cubiculo == null)
            {
                return false;
            }

            int estadoId;
            switch (estado)
            {
                case "Disponible":
                    estadoId = 1;
                    break;
                case "Ocupado":
                    estadoId = 2;
                    break;
                case "Mantenimiento":
                    estadoId = 3;
                    break;
                default:
                    return false; // Estado no válido
            }

            cubiculo.EstadoId = estadoId;
            _context.Cubiculos.Update(cubiculo);
            await _context.SaveChangesAsync();

            // Notificar a los clientes sobre el cambio de estado
            await _hubContext.Clients.All.SendAsync("CubiculoEstadoActualizado", cubiculoId, estado);

            return true;
        }

        public async Task<bool> UpdateMesaEstado(int mesaId, string estado)
        {
            var mesa = await _context.Mesas.FindAsync(mesaId);
            if (mesa == null)
            {
                return false;
            }

            int estadoId;
            switch (estado)
            {
                case "Disponible":
                    estadoId = 1;
                    break;
                case "Ocupado":
                    estadoId = 2;
                    break;
                case "Mantenimiento":
                    estadoId = 3;
                    break;
                default:
                    return false; // Estado no válido
            }

            mesa.EstadoId = estadoId;
            _context.Mesas.Update(mesa);
            await _context.SaveChangesAsync();

            // Notificar a los clientes sobre el cambio de estado
            await _hubContext.Clients.All.SendAsync("MesaEstadoActualizado", mesaId, estado);

            return true;
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

        private async Task CambiarEstadoMesa(int mesaId, int nuevoEstadoId)
        {
            var mesa = await _context.Mesas.FindAsync(mesaId);
            if (mesa != null)
            {
                mesa.EstadoId = nuevoEstadoId;
                _context.Mesas.Update(mesa);
                await _context.SaveChangesAsync();

                // Notificar a los clientes sobre el cambio de estado
                var estado = nuevoEstadoId == 1 ? "Disponible" : "Ocupado"; // Ajustar según los estados disponibles
                await _hubContext.Clients.All.SendAsync("MesaEstadoActualizado", mesaId, estado);
            }
        }
    }
}
