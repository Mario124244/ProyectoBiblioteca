using BibliotecaP.Models.DTO.Cubiculo;
using BibliotecaP.Models.DTO.Mesa;
using BibliotecaP.Services;
using iTextSharp.text.pdf.codec.wmf;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ReservationService _reservationService;

    public ReservationsController(ReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost("cubiculo")]
    public async Task<IActionResult> CreateCubiculoReservation([FromBody] ReservationRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request payload.");
        }

        var reservationId = await _reservationService.CreateCubiculoReservation(request.UserId, request.CubiculoId, request.StartDate, request.EndDate);

        if (reservationId == -1)
        {
            return BadRequest("You already have an active reservation.");
        }

        return Ok(new { reservacionId = reservationId });
    }

    [HttpPost("mesa")]
    public async Task<IActionResult> CreateMesaReservation([FromBody] ReservationRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request payload.");
        }

        var reservationId = await _reservationService.CreateMesaReservation(request.UserId, request.MesaId, request.StartDate, request.EndDate);

        if (reservationId == -1)
        {
            return BadRequest("You already have an active reservation.");
        }

        return Ok(new { reservacionId = reservationId });
    }

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

    [HttpGet("mesas")]
    public async Task<IActionResult> GetMesas()
    {
        var mesas = await _reservationService.GetMesas();

        List<MesaDTO> lista = mesas.Select(x => new MesaDTO
        {
            MesaId = x.MesaId,
            Nombre = x.Nombre,
            EstadoId = x.EstadoId,
            EstadoNombre = x.Estado.Nombre
        }).ToList();
        return Ok(lista);
    }

    [HttpGet("user/cubiculos/{userId}")]
    public async Task<IActionResult> GetUserCubiculoReservations(int userId)
    {
        var reservations = await _reservationService.GetUserCubiculoReservations(userId);
        return Ok(reservations);
    }

    [HttpGet("user/mesas/{userId}")]
    public async Task<IActionResult> GetUserMesaReservations(int userId)
    {
        var reservations = await _reservationService.GetUserMesaReservations(userId);
        return Ok(reservations);
    }
}

public class ReservationRequest
{
    public int UserId { get; set; }
    public int CubiculoId { get; set; }
    public int MesaId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
