using Gym_Boo.Data.Entities;

namespace Gym_Boo.Data.Repositories;

public interface IPlanRepository
{
    Task<IReadOnlyCollection<SubscriptionPlan>> GetAllPlansAsync();

    Task<SubscriptionPlan?> GetPlanByIdAsync(int planId);

    Task<MemberSubscription?> GetActiveSubscriptionByMemberIdAsync(int memberId);

    Task AddSubscriptionAsync(MemberSubscription subscription);

    void UpdateSubscription(MemberSubscription subscription);

    void RemoveSubscription(MemberSubscription subscription);

    Task SaveChangesAsync();
}