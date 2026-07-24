using Gym_Boo.ControllerApi.DTOs;
using Gym_Boo.Data.Entities;

namespace Gym_Boo.ControllerApi.Services;

public interface IPlanService
{
    Task<IReadOnlyList<PlanDto>> GetAllPlansAsync();
}