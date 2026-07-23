using Gym_Boo.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Boo.Controllers.Instructor;

[ApiController]
[Route("api/instructor")]
//[Authorize(Roles = "Instructor")]
public class InstructorControls(IInstructorServices instructorServices) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ShowInstructor(int id, CancellationToken ct)
    {
        var instructor = await instructorServices.GetInstructor(id, ct);
        
        if (instructor is null) 
            return NotFound($"Instructor with ID {id} not found.");

        return Ok(instructor);
    }

    [HttpPost("sessions")]
    public async Task<IActionResult> CreateSession([FromBody] NewSessionDto dto, CancellationToken ct)
    {
        // 1. Basic time validation
        if (dto.EndTime <= dto.StartTime)
        {
            return BadRequest("Session end time must be after the start time.");
        }

        // 2. Map DTO to entity
        var session = new Session
        {
            Start = dto.StartTime,
            End = dto.EndTime,
            Slots = dto.Slots,
            CancellationFee = dto.CancellationFee,
            ClassId = dto.ClassId,
            InstructorId = dto.InstructorId,
            PlaceId = dto.PlaceId
        };
        
        var success = await instructorServices.NewSession(session, ct);
        if (!success)
        {
            return BadRequest("Failed to create session. Check schedule for collisions or invalid references.");
        }
        
        // 3. Return 201 Created with the generated entity
        return CreatedAtAction(nameof(ShowInstructor), new { id = session.InstructorId }, session);
    }
    
    [HttpGet("sessions/{id:int}/attendance")]
    public async Task<IActionResult> GetAttendance(int id, CancellationToken ct)
    {
        var attendance = await instructorServices.GetAttendance(id, ct);
        
        if (attendance is null)
        {
            return NotFound($"Session with ID {id} not found.");
        }

        return Ok(attendance);
    }
}