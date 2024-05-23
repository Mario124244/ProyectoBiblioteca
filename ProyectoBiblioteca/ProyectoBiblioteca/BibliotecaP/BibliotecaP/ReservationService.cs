﻿using BibliotecaP.Models.dbModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaP.Services
{
    public class ReservationService
    {
        private readonly BibliotecaContext _context;

        public ReservationService(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<int> CreateReservation(int userId, int cubiculoId, DateTime startDate, DateTime endDate)
        {
            // Create the reservation
            var reservation = new ReservacionCubiculo
            {
                UsuarioId = userId,
                CubiculoId = cubiculoId,
                FechaHoraInicio = startDate,
                FechaHoraFin = endDate
            };

            _context.ReservacionCubiculos.Add(reservation);

            // Update the state of the cubiculo
            var cubiculo = await _context.Cubiculos.FindAsync(cubiculoId);
            if (cubiculo != null)
            {
                cubiculo.EstadoId = 2; // Asegúrate que el 2 corresponde a 'Ocupado'
                _context.Cubiculos.Update(cubiculo);
                await _context.SaveChangesAsync();  // Esto debe ejecutarse correctamente
            }

            await _context.SaveChangesAsync();

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
    }

}



