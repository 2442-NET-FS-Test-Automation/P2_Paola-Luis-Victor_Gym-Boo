using Gym_Boo.ControllerApi.Dtos;
using Gym_Boo.ControllerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Boo.ControllerApi.Controllers;

[ApiController]
[Route("api/[controller]")] // Maps to /api/reservations
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
    {
        try
        {
            var enrollment = await _reservationService.ReserveClassAsync(dto);

            // 201 Created
            return CreatedAtAction(
                actionName: "GetById",
                controllerName: "Reservations",
                routeValues: new { id = enrollment.Id },
                value: enrollment
            );
        }
        catch (InvalidOperationException ex)
        {
            // 400 Bad Request si fallan las validaciones de negocio (cupo lleno o duplicado)
            return BadRequest(new { message = ex.Message });
        }
    }
}