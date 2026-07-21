using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Boo.Controllers.Administration;

[ApiController]
[Route("controllers/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminServices _adminServices;

    public AdminController(IAdminServices adminServices)
    {
        _adminServices = adminServices;
    }

    // --- DISCIPLINES MANAGEMENT ---

    [HttpPost("disciplines")]
    public async Task<IActionResult> CreateDiscipline([FromBody] string name)
    {
        var result = await _adminServices.NewDisciplineAsync(name);
        if (!result) 
            return BadRequest("Discipline could not be created. It may already exist or the name is invalid.");

        return Ok(new { message = "Discipline created successfully." });
    }

    [HttpPut("disciplines/{id:int}")]
    public async Task<IActionResult> UpdateDiscipline(int id, [FromBody] string newName)
    {
        var result = await _adminServices.UpdateDiscipline(id, newName);
        if (!result) 
            return NotFound($"Discipline with ID {id} not found or invalid name provided.");

        return Ok(new { message = "Discipline updated successfully." });
    }

    [HttpPatch("disciplines/{id:int}/toggle-status")]
    public async Task<IActionResult> ToggleDisciplineStatus(int id)
    {
        var result = await _adminServices.DisableDiscipline(id);
        if (!result) 
            return NotFound($"Discipline with ID {id} not found.");

        return Ok(new { message = "Discipline availability status toggled successfully." });
    }

    [HttpDelete("disciplines")]
    public async Task<IActionResult> DeleteDiscipline([FromQuery] string name)
    {
        var result = await _adminServices.DeleteDiscipline(name);
        if (!result) 
            return NotFound($"Discipline '{name}' not found.");

        return Ok(new { message = "Discipline deleted completely from database." });
    }


    // --- INSTRUCTORS MANAGEMENT ---

    [HttpGet("instructors/{id:int}")]
    public async Task<IActionResult> GetInstructor(int id)
    {
        var instructor = await _adminServices.GetInstructor(id);
        if (instructor == null) 
            return NotFound($"Instructor with ID {id} not found.");

        return Ok(instructor);
    }

    [HttpPost("instructors")]
    public async Task<IActionResult> CreateInstructor([FromBody] User instructor)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        instructor.Role = Role.Instructor; 

        var result = await _adminServices.NewInstructor(instructor);
        if (!result) 
            return BadRequest("Could not add instructor. A user with that email already exists.");

        return CreatedAtAction(nameof(GetInstructor), new { id = instructor.Id }, instructor);
    }

    [HttpPut("instructors")]
    public async Task<IActionResult> UpdateInstructor([FromBody] User instructor)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _adminServices.UpdateInstructor(instructor);
        if (!result) 
            return NotFound($"Instructor with ID {instructor.Id} not found.");

        return Ok(new { message = "Instructor details updated successfully." });
    }

    [HttpDelete("instructors/{id:int}")]
    public async Task<IActionResult> RemoveInstructor(int id)
    {
        var result = await _adminServices.DeleteInstructor(id);
        if (!result) 
            return NotFound($"Instructor with ID {id} not found.");

        return Ok(new { message = "Instructor removed successfully." });
    }
}