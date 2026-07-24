using Gym_Boo.ControllerApi.DTOs;
using Gym_Boo.ControllerApi.Services;

using Microsoft.AspNetCore.Mvc;

namespace GymBoo.ControllerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{
    private readonly IPlanService _planService;

    public PlansController(IPlanService planService)
    {
        _planService = planService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PlanDto>>> Get()
    {
        var plans = await _planService.GetAllPlansAsync();
        return Ok(plans);
    }
}