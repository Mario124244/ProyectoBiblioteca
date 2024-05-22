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

        public async Task<List<Cubiculo>> GetCubiculos()
        {
            return await _context.Cubiculos.Include(c => c.Estado).ToListAsync();
        }

        public bool HasActiveReservation(int userId)
        {
            return _context.ReservacionCubiculos
                .Any(r => r.UsuarioId == userId && r.FechaHoraFin >= DateTime.Now);
        }

        public async Task<string> CreateReservation(int userId, int cubiculoId, DateTime startDate, DateTime endDate)
        {
            if (HasActiveReservation(userId))
            {
                return "You already have an active reservation.";
            }

            var newReservation = new ReservacionCubiculo
            {
                UsuarioId = userId,
                CubiculoId = cubiculoId,
                FechaHoraInicio = startDate,
                FechaHoraFin = endDate
            };

            _context.ReservacionCubiculos.Add(newReservation);
            await _context.SaveChangesAsync();

            return "Reservation created successfully.";
        }

        public async Task<List<ReservacionCubiculo>> GetReservations()
        {
            return await _context.ReservacionCubiculos.Include(r => r.Cubiculo).Include(r => r.Usuario).ToListAsync();
        }

        public async Task<List<ReservacionCubiculo>> GetUserReservations(int userId)
        {
            return await _context.ReservacionCubiculos
                                 .Where(r => r.UsuarioId == userId)
                                 .OrderByDescending(r => r.FechaHoraInicio)
                                 .ToListAsync();
        }
    }
}
