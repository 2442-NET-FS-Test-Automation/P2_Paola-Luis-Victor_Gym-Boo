using System.ComponentModel.DataAnnotations;
using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Data.Entities;
using GymBoo.ControllerApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Boo.Controllers;

[ApiController]
[Route("api/instructor")]
//[Authorize(Roles = "Instructor")]
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
        return instructor != null ? Ok(instructor) : NotFound($"Instructor with ID {id} was not found.");
    }

    [HttpPost("sessions")]
    public async Task<IActionResult> CreateSession([FromBody] NewSessionDto nSession, CancellationToken ct)
    {
        try
        {
            await _instructorService.NewSession(nSession, ct);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpGet("sessions/{id:int}/attendance")]
    public async Task<IActionResult> GetAttendance(int id, CancellationToken ct)
    {
        var attendance = await _instructorService.GetAttendance(id, ct);
        return Ok(attendance);
    }

    public record tmpDTO (int insId);
    [HttpGet("sessions/upcoming-sessions")]
    public async Task<IActionResult> GetUpcomingSessions([FromQuery] tmpDTO name, CancellationToken ct)
    {
        var sessions = await _instructorService.GetUpcomingSessionsForInstructor(name.insId, ct);
        return Ok(sessions);
    }

    [HttpGet("options/classes")]
    public async Task<IActionResult> GetClassOptions(CancellationToken ct)
    {
        return Ok(await _instructorService.GetClassOptions(ct));
    }

    [HttpGet("options/places")]
    public async Task<IActionResult> GetPlaceOptions(CancellationToken ct)
    {
        return Ok(await _instructorService.GetPlaceOptions(ct));
    }

    [HttpDelete("sessions/delete")]
    public async Task<IActionResult> DeleteSession(int id, CancellationToken ct)
    {
        try
        {
            await _instructorService.DeleteSession(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("enrollments/toggle-attendance")]
    public async Task<IActionResult> TakeAttendance([FromBody] TakingAttendanceDTO dto, CancellationToken ct)
    {
        try
        {
            await _instructorService.TakeAttendance(dto, ct);
            return Ok(new { message = "Attendance updated successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}