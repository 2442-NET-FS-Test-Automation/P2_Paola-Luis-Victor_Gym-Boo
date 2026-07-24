using Gym_Boo.ControllerApi.DTOs;
using Gym_Boo.Data.Repositories;

namespace Gym_Boo.ControllerApi.Services;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _planRepo;

    public PlanService(IPlanRepository planRepo)
    {
        _planRepo = planRepo;
    }

    public async Task<IReadOnlyList<PlanDto>> GetAllPlansAsync()
    {
        var plans = await _planRepo.GetAllPlansAsync();

        return plans.Select(p => new PlanDto(
            Id: p.Id,
            Name: p.Name,
            Price: p.Price,
            Recurrence: p.Recurrence.ToString()
        )).ToList();
    }
}