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

    public async Task<bool> EnrollmentHasBeenAttendedAsync(int enrollmentId)
    {
        return await _context.Enrollments
                .AsNoTracking()
                .AnyAsync(e => e.Id == enrollmentId
                && e.Status == EnrollmentStatus.Attended);
    }

    public async Task<Enrollment?> GetByIdWithSessionAsync(int enrollmentId)
    {
        return await _context.Enrollments
        .Include(e => e.Session)
        .FirstOrDefaultAsync(e => e.Id == enrollmentId);
    }

    public async Task<IReadOnlyList<Enrollment>> GetByUserIdAsync(int userId)
    {
        return await _context.Enrollments
        .AsNoTracking()
        .Include(e => e.Session)
            .ThenInclude(s => s.Class)
                .ThenInclude(c => c.Discipline)
        .Include(e => e.Session)
            .ThenInclude(s => s.Instructor)
        .Include(e => e.Session)
            .ThenInclude(s => s.Place)

        .Where(e => e.MemberId == userId)
        .OrderByDescending(e => e.Session.Start)
        .ToListAsync();
    }

    public async Task<bool> MemberHasConflictReservationAsync(int userId, DateTime startTime, DateTime endTime)
    {
        return await _context.Enrollments
        .AnyAsync(e => e.MemberId == userId
                    && e.Status == EnrollmentStatus.Enrolled
                    && e.Session.Start < endTime
                    && e.Session.End > startTime);
    }

    public Task UpdateAsync(Enrollment enrollment)
    {
        _context.Enrollments.Update(enrollment);
        return Task.CompletedTask;
    }
}