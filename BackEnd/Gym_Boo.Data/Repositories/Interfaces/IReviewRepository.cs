using Gym_Boo.Data.DTOs;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;

namespace Gym_Boo.Data.Repositories;

public interface IReviewRepository
{
    Task AddAsync(Review review);

    Task<IReadOnlyList<Review>> GetReviewsByEnrollment(int enrollmentId);

    Task<bool> ExistAsync(int enrollmentId, ReviewType reviewType);
}