using Gym_Boo.Data.Entities;

namespace Gym_Boo.Data.Repositories;

public interface IReviewRepository
{
    Task AddAsync(Review review);
}