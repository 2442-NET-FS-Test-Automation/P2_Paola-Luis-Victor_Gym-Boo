using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Controllers.Services;

public class InstructorServices : IInstructorServices
{
    private readonly GymBooDbContext _db;

    public InstructorServices(GymBooDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetInstructor(int id, CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<bool> NewSession(Session session, CancellationToken ct)
    {
        if (session.Id != 0 && await _db.Sessions.AnyAsync(s => s.Id == session.Id, ct))
        {
            return false;
        }

        await _db.Sessions.AddAsync(session, ct);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<SessionAttendanceResponseDto> GetAttendance(int id, CancellationToken ct)
    {
        var subscribers = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.SessionId == id && e.Status == EnrollmentStatus.Enrolled)
            .Select(e => new SubscriberDto(
                e.MemberId,
                e.Member.Email
            ))
            .ToListAsync(ct);

        return new SessionAttendanceResponseDto(
            SessionId: id,
            TotalEnrolled: subscribers.Count,
            Subscribers: subscribers
        );
    }
}