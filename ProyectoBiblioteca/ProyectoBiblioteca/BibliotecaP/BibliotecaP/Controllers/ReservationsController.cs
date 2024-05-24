using BibliotecaP.Models.DTO.Cubiculo;
using BibliotecaP.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ReservationService _reservationService;

    // Constructor: Inicializa una nueva instancia del ReservationsController
    // y asigna el servicio de reservas (ReservationService) a una variable privada.
    public ReservationsController(ReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    // Método CreateReservation: Crea una nueva reserva a partir de la solicitud enviada en el cuerpo de la petición.
    // Valida la solicitud, invoca el servicio de creación de reservas y devuelve una respuesta adecuada según el resultado.
    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request payload.");
        }

        var reservationId = await _reservationService.CreateReservation(request.UserId, request.CubiculoId, request.StartDate, request.EndDate);

        if (reservationId == -1)
        {
            return BadRequest("You already have an active reservation.");
        }

        return Ok(new { reservacionId = reservationId });
    }

    // Método GetCubiculos: Recupera la lista de cubículos.
    // Llama al servicio de reservas para obtener la lista de cubículos y devolverla en la respuesta.
    [HttpGet("cubiculos")]
    public async Task<IActionResult> GetCubiculos()
    {
        var cubiculos = await _reservationService.GetCubiculos();

        List<CubiculoDTO> lista = cubiculos.Select(x => new CubiculoDTO
        {
            CubiculoId = x.CubiculoId,
            Nombre = x.Nombre,
            EstadoId = x.EstadoId,
            EstadoNombre = x.Estado.Nombre
        }).ToList();
        return Ok(lista);
    }

    // Método GetUserReservations: Recupera las reservas del usuario especificado.
    // Obtiene y devuelve las reservas del usuario llamando al servicio de reservas.
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserReservations(int userId)
    {
        var reservations = await _reservationService.GetUserReservations(userId);
        return Ok(reservations);
    }
}

// Clase ReservationRequest: Representa la solicitud de creación de una reserva.
public class ReservationRequest
{
    public int UserId { get; set; }
    public int CubiculoId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
