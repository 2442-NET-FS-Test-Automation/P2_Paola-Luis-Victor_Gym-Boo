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

    public async Task<Member?> GetByIdWithReportDataAsync(int memberId)
    {
        return await _context.Members
            .Include(m => m.MemberSubscription)
            .Include(m => m.Enrollments)
                .ThenInclude(e => e.Reviews)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == memberId);
    }
}