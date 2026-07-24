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

    public async Task<IReadOnlyCollection<SubscriptionPlan>> GetAllPlansAsync()
    {
        return await _db.SubscriptionPlans.ToListAsync();
    }
}