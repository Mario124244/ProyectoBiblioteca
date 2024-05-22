using BibliotecaP.Services;
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

        return Ok(new { reservacionId = reservationId }); // Return the reservation ID
    }

    [HttpGet("cubiculos")]
    public async Task<IActionResult> GetCubiculos()
    {
        var cubiculos = await _reservationService.GetCubiculos();
        return Ok(cubiculos);
    }

    [HttpGet("reservations")]
    public async Task<IActionResult> GetReservations()
    {
        var reservations = await _reservationService.GetReservations();
        return Ok(reservations);
    }

    

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserReservations(int userId)
    {
        var reservations = await _reservationService.GetUserReservations(userId);
        return Ok(reservations);
    }

}

public class ReservationRequest
{
    public int UserId { get; set; }
    public int CubiculoId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
