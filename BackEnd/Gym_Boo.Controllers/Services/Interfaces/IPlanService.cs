using Gym_Boo.ControllerApi.DTOs;
using Gym_Boo.Data.Entities;

namespace Gym_Boo.ControllerApi.Services;

public interface IPlanService
{
    Task<IReadOnlyList<PlanDto>> GetAllPlansAsync();

    Task<OperationResultDTO> SuscribePlan(int MemberId, int PlanId);

    Task<OperationResultDTO> UpdatePlanSubs(int MemberId, int currentPlanId, int newPlanId);

    Task<OperationResultDTO> UnsubscribePlan(int MemberId);
}