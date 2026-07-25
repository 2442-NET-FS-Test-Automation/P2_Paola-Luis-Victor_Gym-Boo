using Gym_Boo.ControllerApi.DTOs;
using Gym_Boo.Data.Entities;
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

    public async Task<OperationResultDTO> SuscribePlan(int memberId, int planId)
    {
        if (memberId <= 0 || planId <= 0)
            throw new ArgumentException("Invalid MemberId or PlanId.");

        var plan = await _planRepo.GetPlanByIdAsync(planId);
        if (plan is null)
            throw new InvalidOperationException($"Plan with ID {planId} was not found.");

        var existingSub = await _planRepo.GetActiveSubscriptionByMemberIdAsync(memberId);
        if (existingSub is not null)
            throw new InvalidOperationException("Member already has an active subscription.");

        var now = DateTime.UtcNow;
        var newSubscription = new MemberSubscription
        {
            MemberId = memberId,
            PlanId = planId,
            StartDate = now,
            ExpirationDate = CalculateExpirationDate(now, plan.Recurrence.ToString())
        };

        await _planRepo.AddSubscriptionAsync(newSubscription);
        await _planRepo.SaveChangesAsync();

        return new OperationResultDTO(true, "Subscription created successfully.");
    }

    public async Task<OperationResultDTO> UnsubscribePlan(int memberId)
    {
        if (memberId <= 0)
            throw new ArgumentException("Invalid MemberId.");

        var activeSub = await _planRepo.GetActiveSubscriptionByMemberIdAsync(memberId);
        if (activeSub is null)
            throw new InvalidOperationException("Member does not have an active subscription to cancel.");

        _planRepo.RemoveSubscription(activeSub);
        await _planRepo.SaveChangesAsync();

        return new OperationResultDTO(true, "Subscription cancelled successfully.");
    }

    public async Task<OperationResultDTO> UpdatePlanSubs(int memberId, int currentPlanId, int newPlanId)
    {
        if (memberId <= 0 || currentPlanId <= 0 || newPlanId <= 0)
            throw new ArgumentException("Invalid arguments provided for update.");

        if (currentPlanId == newPlanId)
            throw new ArgumentException("New plan must be different from the current plan.");

        var currentSub = await _planRepo.GetActiveSubscriptionByMemberIdAsync(memberId);
        if (currentSub is null || currentSub.PlanId != currentPlanId)
            throw new InvalidOperationException("Active subscription matching currentPlanId was not found.");

        var newPlan = await _planRepo.GetPlanByIdAsync(newPlanId);
        if (newPlan is null)
            throw new InvalidOperationException($"New plan with ID {newPlanId} was not found.");

        // Actualizamos plan y recalculamos fecha de expiración partiendo de hoy
        var now = DateTime.UtcNow;
        currentSub.PlanId = newPlanId;
        currentSub.StartDate = now;
        currentSub.ExpirationDate = CalculateExpirationDate(now, newPlan.Recurrence.ToString());

        _planRepo.UpdateSubscription(currentSub);
        await _planRepo.SaveChangesAsync();

        return new OperationResultDTO(true, "Subscription updated successfully.");
    }

    private DateTime CalculateExpirationDate(DateTime startDate, string recurrence)
    {
        return recurrence?.ToLower() switch
        {
            "annual" or "yearly" => startDate.AddYears(1),
            "quarterly" => startDate.AddMonths(3),
            _ => startDate.AddMonths(1)
        };
    }
}