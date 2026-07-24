using Gym_Boo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Data.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly GymBooDbContext _db;

    public PlanRepository(GymBooDbContext db)
    {
        _db = db;
    }

    public async Task AddSubscriptionAsync(MemberSubscription subscription)
    {
        await _db.MemberSubscriptions.AddAsync(subscription);
    }

    public async Task<MemberSubscription?> GetActiveSubscriptionByMemberIdAsync(int memberId)
    {
        return await _db.MemberSubscriptions
            .FirstOrDefaultAsync(ms => ms.MemberId == memberId);
    }

    public async Task<IReadOnlyCollection<SubscriptionPlan>> GetAllPlansAsync()
    {
        return await _db.SubscriptionPlans.ToListAsync();
    }

    public async Task<SubscriptionPlan?> GetPlanByIdAsync(int planId)
    {
        return await _db.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == planId);
    }

    public void RemoveSubscription(MemberSubscription subscription)
    {
        _db.MemberSubscriptions.Remove(subscription);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }

    public void UpdateSubscription(MemberSubscription subscription)
    {
        _db.MemberSubscriptions.Update(subscription);
    }
}