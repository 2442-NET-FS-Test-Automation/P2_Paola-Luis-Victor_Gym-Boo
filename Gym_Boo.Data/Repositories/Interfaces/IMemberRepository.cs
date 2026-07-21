using Gym_Boo.Data.Entities;

namespace Gym_Boo.Data.Repositories;

public interface IMemberRepository
{
    Task<Member?> GetByIdWithSubscriptionAsync(int memberId);
}