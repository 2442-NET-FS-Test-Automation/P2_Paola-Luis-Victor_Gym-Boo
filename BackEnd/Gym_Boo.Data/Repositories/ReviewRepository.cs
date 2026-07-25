using Gym_Boo.Data.DTOs;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Data.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly GymBooDbContext _context;

    public ReviewRepository(GymBooDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
    }

    public async Task<bool> ExistAsync(int enrollmentId, ReviewType reviewType)
    {
        return await _context.Reviews
        .AnyAsync(r => r.EnrollmentId == enrollmentId && r.ReviewType == reviewType);
    }

    public async Task<IReadOnlyList<Review>> GetReviewsByEnrollment(int enrollmentId)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => r.EnrollmentId == enrollmentId)
            .ToListAsync();
    }
}