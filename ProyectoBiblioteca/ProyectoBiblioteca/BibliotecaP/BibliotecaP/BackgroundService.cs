using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BibliotecaP.Models.dbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

public class CubiculoBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public CubiculoBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ActualizarEstadoCubiculos();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Verifica cada minuto
        }
    }

    private async Task ActualizarEstadoCubiculos()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BibliotecaContext>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<CubiculoHub>>();

            var ahora = DateTime.UtcNow;

            // Obtener todas las reservas que han terminado
            var reservasExpiradas = await context.ReservacionCubiculos
                .Where(r => r.FechaHoraFin <= ahora && r.Cubiculo.EstadoId != 1) // Asumiendo 1 es 'Disponible'
                .ToListAsync();

            foreach (var reserva in reservasExpiradas)
            {
                var cubiculo = await context.Cubiculos.FindAsync(reserva.CubiculoId);
                if (cubiculo != null)
                {
                    cubiculo.EstadoId = 1; // Cambiar a 'Disponible'
                    context.Cubiculos.Update(cubiculo);
                    await context.SaveChangesAsync();
                    await hubContext.Clients.All.SendAsync("CubiculoEstadoActualizado", cubiculo.CubiculoId, "Disponible");
                }
            }
        }
    }
}
