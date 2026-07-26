using Gym_Boo.ControllerApi.DTOs;

namespace Gym_Boo.ControllerApi.Services;

public interface IMemberService
{
    Task<MemberReportDto?> GetMemberReportAsync(int memberId);
}