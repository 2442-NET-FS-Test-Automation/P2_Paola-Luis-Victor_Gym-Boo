using Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Data.Entities;
using GymBoo.ControllerApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Gym_Boo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Instructor")]
public class InstructorController : ControllerBase
{
    private readonly IInstructorServices _instructorService;

    public InstructorController(IInstructorServices instructorService)
    {
        _instructorService = instructorService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetInstructor(int id, CancellationToken ct)
    {
        var instructor = await _instructorService.GetInstructor(id, ct);
        return instructor != null ? Ok(instructor) : NotFound();
    }

    [HttpPost("sessions")]
    public async Task<IActionResult> CreateSession([FromBody] Session session, CancellationToken ct)
    {
        var result = await _instructorService.NewSession(session, ct);
        return result ? Ok() : BadRequest("Could not create session due to time conflict or invalid data.");
    }

    [HttpGet("sessions/{id:int}/attendance")]
    public async Task<IActionResult> GetAttendance(int id, CancellationToken ct)
    {
        var attendance = await _instructorService.GetAttendance(id, ct);
        return Ok(attendance);
    }

    [HttpGet("{instructorId:int}/upcoming-sessions")]
    public async Task<IActionResult> GetUpcomingSessions(int instructorId, CancellationToken ct)
    {
        var sessions = await _instructorService.GetUpcomingSessionsForInstructor(instructorId, ct);
        return Ok(sessions);
    }

    [HttpGet("class-options")]
    public async Task<IActionResult> GetClassOptions(CancellationToken ct)
    {
        return Ok(await _instructorService.GetClassOptions(ct));
    }

    [HttpGet("place-options")]
    public async Task<IActionResult> GetPlaceOptions(CancellationToken ct)
    {
        return Ok(await _instructorService.GetPlaceOptions(ct));
    }

    [HttpDelete("sessions/{id:int}")]
    public async Task<IActionResult> DeleteSession(int id, CancellationToken ct)
    {
        var result = await _instructorService.DeleteSession(id, ct);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("attendance")]
    public async Task<IActionResult> TakeAttendance([FromBody] TakingAttendanceDTO dto, CancellationToken ct)
    {
        var result = await _instructorService.TakeAttendance(dto, ct);
        return result ? Ok() : BadRequest("Invalid attendance request.");
    }
}