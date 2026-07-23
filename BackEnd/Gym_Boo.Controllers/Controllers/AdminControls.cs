using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Boo.Controllers.Administration;

[ApiController]
[Route("api/admin")]
//[Authorize(Roles = "Admin")]
public class AdminController(IAdminServices adminServices) : ControllerBase
{
    // --- DISCIPLINES MANAGEMENT ---

    [HttpPost("disciplines/create")]
    public async Task<IActionResult> CreateDiscipline([FromBody] DisciplineDto dto, CancellationToken ct)
    {
        var result = await adminServices.NewDisciplineAsync(dto.Name, ct);
        if (!result) 
            return BadRequest("Discipline could not be created. It may already exist or the name is invalid.");

        return Ok(new { message = "Discipline created successfully." });
    }

    [HttpPut("disciplines/{id:int}")]
    public async Task<IActionResult> UpdateDiscipline(int id, [FromBody] DisciplineDto dto, CancellationToken ct)
    {
        var result = await adminServices.UpdateDiscipline(id, dto.Name, ct);
        if (!result) 
            return NotFound($"Discipline with ID {id} not found or invalid name provided.");

        return Ok(new { message = "Discipline updated successfully." });
    }

    [HttpPatch("disciplines/{id:int}/toggle-status")]
    public async Task<IActionResult> ToggleDisciplineStatus(int id, CancellationToken ct)
    {
        var result = await adminServices.DisableDiscipline(id, ct);
        if (!result) 
            return NotFound($"Discipline with ID {id} not found.");

        return Ok(new { message = "Discipline availability status toggled successfully." });
    }

    [HttpDelete("disciplines")]
    public async Task<IActionResult> DeleteDiscipline([FromQuery] string name, CancellationToken ct)
    {
        var result = await adminServices.DeleteDiscipline(name, ct);
        if (!result) 
            return NotFound($"Discipline '{name}' not found.");

        return Ok(new { message = "Discipline deleted completely from database." });
    }


    // --- INSTRUCTORS MANAGEMENT ---

    [HttpGet("instructors/{id:int}")]
    public async Task<IActionResult> GetInstructor(int id, CancellationToken ct)
    {
        var instructor = await adminServices.GetInstructor(id, ct);
        if (instructor is false) 
            return NotFound($"Instructor with ID {id} not found.");

        return Ok(instructor);
    }

    [HttpPost("instructors")]
    public async Task<IActionResult> CreateInstructor([FromBody] CreateInstructorDto dto, CancellationToken ct)
    {
        // Map DTO to User entity
        var instructor = new User 
        { 
            Email = dto.Email, 
            Role = Role.Instructor 
        };

        var result = await adminServices.NewInstructor(instructor, ct);
        if (!result) 
            return BadRequest("Could not add instructor. A user with that email already exists.");

        return CreatedAtAction(nameof(GetInstructor), new { id = instructor.Id }, instructor);
    }

    [HttpPut("instructors/{id:int}")]
    public async Task<IActionResult> UpdateInstructor(int id, [FromBody] User instructor, CancellationToken ct)
    {
        if (id != instructor.Id)
            return BadRequest("ID in route does not match entity ID.");

        var result = await adminServices.UpdateInstructor(instructor, ct);
        if (!result) 
            return NotFound($"Instructor with ID {instructor.Id} not found.");

        return Ok(new { message = "Instructor details updated successfully." });
    }

    [HttpDelete("instructors/{id:int}")]
    public async Task<IActionResult> RemoveInstructor(int id, CancellationToken ct)
    {
        var result = await adminServices.DeleteInstructor(id, ct);
        if (!result) 
            return NotFound($"Instructor with ID {id} not found.");

        return Ok(new { message = "Instructor removed successfully." });
    }
}