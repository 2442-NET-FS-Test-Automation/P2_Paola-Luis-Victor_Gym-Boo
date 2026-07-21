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

    // GET /api/reservations?userId=1
    [HttpGet]
    public async Task<ActionResult<UserReservationsResponseDto>> GetMyReservations([FromQuery] int userId)
    {
        if (userId <= 0)
        {
            return BadRequest(new { message = "You must provide a valid 'userId'." });
        }

        var history = await _reservationService.GetUserReservationHistoryAsync(userId);
        return Ok(history);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
    {
        try
        {
            var enrollmentDto = await _reservationService.ReserveClassAsync(dto);

            return Created(enrollmentDto.Id.ToString(), enrollmentDto);

            // 201 Created (To use later)
            // return CreatedAtAction(
            //     actionName: "GetById",
            //     controllerName: "Reservations",
            //     routeValues: new { id = enrollment.Id },
            //     value: enrollment
            // );
        }
        catch (InvalidOperationException ex)
        {
            // 400 Bad Request si fallan las validaciones de negocio
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE /api/reservations/15?userId=3
    // (OR DELETE /api/reservations/15 obtaining userId from JWT token)
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<CancelReservationResultDto>> Cancel(int id, [FromQuery] int userId)
    {
        try
        {
            var result = await _reservationService.CancelReservationAsync(id, userId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}