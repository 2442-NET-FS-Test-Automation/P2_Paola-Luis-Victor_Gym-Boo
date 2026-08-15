using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Boo.Controllers.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController(IAdminServices adminServices) : ControllerBase
{
    // ==========================================
    // DISCIPLINES MANAGEMENT
    // ==========================================

    [HttpGet("disciplines")]
    public async Task<IActionResult> GetDisciplinesList(CancellationToken ct)
    {
        var result = await adminServices.GetAllDisciplines(ct);
        return Ok(result);
    }

    [HttpPost("disciplines")]
    public async Task<IActionResult> CreateDiscipline([FromBody] DisciplineDto dto, CancellationToken ct)
    {
        var success = await adminServices.NewDisciplineAsync(dto.Name, ct);
        if (!success)
            return BadRequest(new
                { message = "Discipline could not be created. It may already exist or the name is invalid." });

        return StatusCode(StatusCodes.Status201Created, new { message = "Discipline created successfully." });
    }

    [HttpPut("disciplines/{id:int}")]
    public async Task<IActionResult> UpdateDiscipline(int id, [FromBody] DisciplineDto dto, CancellationToken ct)
    {
        var success = await adminServices.UpdateDiscipline(id, dto.Name, ct);
        if (!success)
            return NotFound(new { message = $"Discipline with ID {id} not found or invalid name provided." });

        return Ok(new { message = "Discipline updated successfully." });
    }

    [HttpPatch("disciplines/{id:int}/toggle-status")]
    public async Task<IActionResult> ToggleDisciplineStatus(int id, CancellationToken ct)
    {
        var success = await adminServices.ToggleDiscipline(id, ct);
        if (!success)
            return NotFound(new { message = $"Discipline with ID {id} not found." });

        return Ok(new { message = "Discipline availability status toggled successfully." });
    }

    [HttpDelete("disciplines")]
    public async Task<IActionResult> DeleteDiscipline([FromQuery] string name, CancellationToken ct)
    {
        var success = await adminServices.DeleteDiscipline(name, ct);
        if (!success)
            return NotFound(new { message = $"Discipline '{name}' not found." });

        return Ok(new { message = "Discipline deleted completely from database." });
    }

    // ==========================================
    // INSTRUCTORS MANAGEMENT
    // ==========================================

    [HttpGet("instructors")]
    public async Task<IActionResult> GetInstructors(CancellationToken ct)
    {
        var instructors = await adminServices.GetAllInstructors(ct);
        return Ok(instructors);
    }

    [HttpGet("instructors/{id:int}")]
    public async Task<IActionResult> GetInstructor(int id, CancellationToken ct)
    {
        // NOTE: Ensure GetInstructor in IAdminServices returns Task<User?> instead of Task<bool>
        var instructorExists = await adminServices.GetInstructor(id, ct);
        if (!instructorExists)
            return NotFound(new { message = $"Instructor with ID {id} not found." });

        return Ok(instructorExists);
    }

    [HttpPost("instructors")]
    public async Task<IActionResult> CreateInstructor([FromBody] CreateInstructorDto dto, CancellationToken ct)
    {
        // Map DTO to domain model inside controller (or pass DTO directly to service)
        var instructor = new User
        {
            Email = dto.Email,
            Name = dto.FirstName,
            LastName = dto.LastName,
            IsActive = true
        };

        var success = await adminServices.NewInstructor(instructor, ct);
        if (!success)
            return BadRequest(new { message = "Could not add instructor. A user with that email already exists." });

        return StatusCode(StatusCodes.Status201Created, new { message = "Instructor created successfully." });
    }

    [HttpPut("instructors/{id:int}")]
    public async Task<IActionResult> UpdateInstructor(int id, [FromBody] UpdateInstructorDto dto, CancellationToken ct)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID in route does not match body ID." });

        var instructor = new User
        {
            Id = dto.Id,
            Name = dto.Name,
            LastName = dto.LastName,
            Email = dto.Email
        };

        var success = await adminServices.UpdateInstructor(instructor, ct);
        if (!success)
            return NotFound(new { message = $"Instructor with ID {id} not found." });

        return Ok(new { message = "Instructor details updated successfully." });
    }

    [HttpDelete("instructors/{id:int}")]
    public async Task<IActionResult> RemoveInstructor(int id, CancellationToken ct)
    {
        var success = await adminServices.DeleteInstructor(id, ct);
        if (!success)
            return NotFound(new { message = $"Instructor with ID {id} not found." });

        return Ok(new { message = "Instructor removed successfully." });
    }

    // ==========================================
    // REPORTS & ANALYTICS
    // ==========================================

    [HttpGet("reports/sessions")]
    public async Task<IActionResult> GetSessionsReport(CancellationToken ct)
    {
        var report = await adminServices.RegistrationReports(ct);
        if (report is null || !report.Any())
            return NotFound(new { message = "No sessions found to generate the report." });

        return Ok(report);
    }

    [HttpGet("reports/revenue")]
    public async Task<IActionResult> GetRevenueReport(CancellationToken ct)
    {
        var report = await adminServices.TotalRevenue(ct);

        var response = new
        {
            CancellationRevenue = report[0],
            SubscriptionRevenue = report[1],
            TotalRevenue = report[2]
        };

        return Ok(response);
    }

    [HttpGet("reports/best-rated")]
    public async Task<IActionResult> GetMostPopularReport(CancellationToken ct)
    {
        var sessions = await adminServices.MostPopularClass(ct);
        if (!sessions.Any())
            return NoContent();

        return Ok(sessions);
    }
}