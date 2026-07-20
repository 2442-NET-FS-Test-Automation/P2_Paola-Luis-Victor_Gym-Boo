using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Data.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly GymBooDbContext _context;

    public EnrollmentRepository(GymBooDbContext context)
    {
        _context = context;
    }

    public async Task AddEnrollmentAsync(Enrollment enrollment)
    {
        await _context.Enrollments.AddAsync(enrollment);
    }

    public async Task<bool> MemberHasConflictReservationAsync(int userId, DateTime startTime, DateTime endTime)
    {
        return await _context.Enrollments
        .AnyAsync(e => e.MemberId == userId
                    && e.Status == EnrollmentStatus.Enrolled
                    && e.Session.Start < endTime
                    && e.Session.End > startTime);
    }
}