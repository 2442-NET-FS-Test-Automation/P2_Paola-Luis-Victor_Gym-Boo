using Gym_Boo.Data.Entities;
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
}