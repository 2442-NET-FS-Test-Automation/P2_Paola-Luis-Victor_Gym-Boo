using Gym_Boo.ControllerApi.DTOs;
using Gym_Boo.ControllerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace GymBoo.ControllerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemberController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MemberController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet("{memberId}/report")]
    public async Task<ActionResult<MemberReportDto>> GetMemberReport(int memberId)
    {
        var report = await _memberService.GetMemberReportAsync(memberId);

        if (report is null)
        {
            return NotFound(new { message = $"Member with ID {memberId} not found." });
        }

        return Ok(report);
    }
}