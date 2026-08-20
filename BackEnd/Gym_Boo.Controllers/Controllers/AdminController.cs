namespace Gym_Boo.Controllers.Controllers;

using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Controllers.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController(IAdminServices adminServices) : ControllerBase
{
    // ==========================================
    // DISCIPLINES
    // ==========================================

    [HttpGet("disciplines/list")]
    public async Task<IActionResult> GetDisciplinesList(CancellationToken ct)
    {
        var result = await adminServices.GetAllDisciplines(ct);
        return Ok(result);
    }

    [HttpPost("disciplines/create")]
    public async Task<IActionResult> CreateDiscipline([FromBody] DisciplineDto dto, CancellationToken ct)
    {
        var discipline = await adminServices.NewDisciplineAsync(dto.Name, ct);
        return StatusCode(StatusCodes.Status201Created, discipline);
    }

    [HttpPut("disciplines/{id:int}")]
    public async Task<IActionResult> UpdateDiscipline(int id, [FromBody] DisciplineDto dto, CancellationToken ct)
    {
        await adminServices.UpdateDiscipline(id, dto.Name, ct);
        return Ok(new { message = "Discipline updated successfully." });
    }

    [HttpPatch("disciplines/{id:int}/toggle-status")]
    public async Task<IActionResult> ToggleDisciplineStatus(int id, CancellationToken ct)
    {
        await adminServices.ToggleDiscipline(id, ct);
        return Ok(new { message = "Discipline availability status toggled successfully." });
    }

    [HttpDelete("disciplines/{name}")]
    public async Task<IActionResult> DeleteDiscipline(string name, CancellationToken ct)
    {
        await adminServices.DeleteDiscipline(name, ct);
        return Ok(new { message = "Discipline deleted completely from database." });
    }

    // ==========================================
    // INSTRUCTORS
    // ==========================================

    [HttpGet("instructors/list")]
    public async Task<IActionResult> GetInstructors(CancellationToken ct)
    {
        var instructors = await adminServices.GetAllInstructors(ct);
        return Ok(instructors);
    }

    [HttpGet("instructors/{id:int}")]
    public async Task<IActionResult> GetInstructor(int id, CancellationToken ct)
    {
        var instructor = await adminServices.GetInstructorById(id, ct);
        return Ok(instructor);
    }

    [HttpPost("instructors/create")]
    public async Task<IActionResult> CreateInstructor([FromBody] Data.DTOs.CreateInstructorDto dto, CancellationToken ct)
    {
        var createdInstructor = await adminServices.NewInstructor(dto, ct);
        return CreatedAtAction(nameof(GetInstructor), new { id = createdInstructor.Id }, createdInstructor);
    }

    [HttpPut("instructors/{id:int}")]
    public async Task<IActionResult> UpdateInstructor(int id, [FromBody] Data.DTOs.UpdateInstructorDto dto, CancellationToken ct)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID in route does not match body ID." });

        await adminServices.UpdateInstructor(dto, ct);
        return Ok(new { message = "Instructor details updated successfully." });
    }

    [HttpDelete("instructors/{id:int}")]
    public async Task<IActionResult> RemoveInstructor(int id, CancellationToken ct)
    {
        await adminServices.DeleteInstructor(id, ct);
        return Ok(new { message = "Instructor removed successfully." });
    }

    // ==========================================
    // REPORTS & ANALYTICS
    // ==========================================

    [HttpGet("reports/sessions")]
    public async Task<IActionResult> GetSessionsReport(CancellationToken ct)
    {
        var report = await adminServices.RegistrationReports(ct);
        return Ok(report);
    }

    [HttpGet("reports/revenue")]
    public async Task<IActionResult> GetRevenueReport(CancellationToken ct)
    {
        var report = await adminServices.TotalRevenue(ct);
        return Ok(report);
    }

    [HttpGet("reports/bestrated")]
    public async Task<IActionResult> GetMostPopularReport(CancellationToken ct)
    {
        var sessions = await adminServices.MostPopularClass(ct);
        return Ok(sessions);
    }
}