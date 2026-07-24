using Gym_Boo.Data.DTOs;
using Gym_Boo.ControllerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Boo.ControllerApi.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/classes
public class ClassesController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public ClassesController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    // GET /api/classes?discipline=yoga&date=2026-07-15
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClassSessionDto>>> Get([FromQuery] string? discipline, [FromQuery] DateTime? date, [FromQuery] bool past = false)
    {
        var classes = await _sessionService.GetFilteredSessionsAsync(discipline, date, past);
        return Ok(classes);
    }

}