using Gym_Boo.ControllerApi.DTOs;
using Gym_Boo.ControllerApi.Exceptions;
using Gym_Boo.ControllerApi.Services;

using Microsoft.AspNetCore.Mvc;

namespace Gym_Boo.ControllerApi.Controllers;

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

    [HttpPost("subscription/new")]
    public async Task<ActionResult> SubscribePlan([FromBody] MemberPlanChoiceDto dto)
    {
        try
        {
            var createdOperationResultDto = await _planService.SuscribePlan(dto.MemberId, dto.PlanId);
            return Ok(createdOperationResultDto);
        } catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message }); 
        } catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("subscription/update")]
    public async Task<ActionResult> UpdatePlan([FromBody] MemberPlanUpdateDto dto)
    {
        try
        {
            var updatedOperationResultDto =  await _planService.UpdatePlanSubs(dto.MemberId, dto.CurrentPlanId, dto.NewPlanId);
            return Ok(updatedOperationResultDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("subscription/cancel/{memberId}")] 
    public async Task<ActionResult> CancelPlan(int memberId)
    {
        try
        {
            await _planService.UnsubscribePlan(memberId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}