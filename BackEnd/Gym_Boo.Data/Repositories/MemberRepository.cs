using Gym_Boo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Data.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly GymBooDbContext _context;

    public MemberRepository(GymBooDbContext context)
    {
        _context = context;
    }

    public async Task<Member?> GetByIdWithSubscriptionAsync(int memberId)
    {
        return await _context.Members
        .Include(m => m.MemberSubscription)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == memberId);
    }
}