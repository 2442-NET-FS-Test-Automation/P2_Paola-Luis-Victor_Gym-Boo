using Gym_Boo.Data.Entities;

namespace Gym_Boo.Data.Repositories;

public interface IPlanRepository
{
    Task<IReadOnlyCollection<SubscriptionPlan>> GetAllPlansAsync();
}