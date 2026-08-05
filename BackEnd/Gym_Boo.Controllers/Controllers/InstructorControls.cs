using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Data.Entities;
using GymBoo.ControllerApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Gym_Boo.Controllers.Controllers;

[ApiController]
[Route("api/instructor")]
//[Authorize(Roles = "Instructor")]
public class InstructorControls(IInstructorServices instructorServices) : ControllerBase
{
    // Gets the profile information for a specific instructor by ID.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ShowInstructor(int id, CancellationToken ct)
    {
        var instructor = await instructorServices.GetInstructor(id, ct);
        
        if (instructor is null) 
            return NotFound($"Instructor with ID {id} not found.");

        return Ok(instructor);
    }

    // Creates a new class session for an instructor.
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

    // Retrieves the attendance list for a specific session.
    [HttpGet("sessions/{id:int}/attendance")]
    public async Task<IActionResult> GetAttendance(int id, CancellationToken ct)
    {
        var attendance = await instructorServices.GetAttendance(id, ct);
        
        if (attendance is null)
        {
            return NotFound($"No sessions found.");
        }

        return Ok(attendance);
    }

    // Retrieves all upcoming sessions assigned to an instructor
    [HttpGet("sessions/list")]
    public async Task<IActionResult> GetNextSessions(int insId, CancellationToken ct)
    {
        var next = await instructorServices.GetUpcomingSessionsForInstructor(insId, ct);
        
        if (next is null || !next.Any())
        {
            return NotFound("No upcoming sessions.");
        }
    
        return Ok(next);
    }

    // Retrieves all available classes for the session creation dropdown.
    [HttpGet("options/classes")]
    public async Task<IActionResult> GetClassOptions(CancellationToken ct)
    {
        var classes =
            await instructorServices.GetClassOptions(ct);

        return Ok(classes);
    }

    // Retrieves all available places for the session creation dropdown.
    [HttpGet("options/places")]
    public async Task<IActionResult> GetPlaceOptions(CancellationToken ct)
    {
        var places =
            await instructorServices.GetPlaceOptions(ct);

        return Ok(places);
    }

    // Deletes an existing session by its ID.
    [HttpDelete("sessions/delete")]
    public async Task<IActionResult> DeleteSession(int id, CancellationToken ct)
    {
        var res = await instructorServices.DeleteSession(id, ct);

        if (res is false)
        {
            return null;
        }

        return Ok(res);
    }

    [HttpPatch("enrollments/toggle-attendance")]
    public async Task<IActionResult> TakeAttendance(TakingAttendanceDTO takingAttendanceDTO)
    {
        try
        {
            bool result = await instructorServices.TakeAttendance(takingAttendanceDTO);

            return Ok(result);
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
