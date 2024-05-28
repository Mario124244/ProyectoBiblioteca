using Microsoft.AspNetCore.SignalR;

public class CubiculoHub : Hub
{
    public async Task CubiculoEstadoActualizado(int cubiculoId, string estado)
    {
        await Clients.All.SendAsync("CubiculoEstadoActualizado", cubiculoId, estado);
    }
}
