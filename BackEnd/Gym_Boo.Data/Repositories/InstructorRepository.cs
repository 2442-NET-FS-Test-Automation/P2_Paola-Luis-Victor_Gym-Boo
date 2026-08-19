using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Data.Repositories;

public class InstructorRepository : IInstructorRepository
{
    private readonly GymBooDbContext _db;

    public InstructorRepository(GymBooDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetUserByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<bool> HasSessionOverlapAsync(int placeId, int instructorId, DateTime start, DateTime end,
        CancellationToken ct)
    {
        return await _db.Sessions.AnyAsync(s =>
                (s.PlaceId == placeId || s.InstructorId == instructorId) &&
                start < s.End &&
                end > s.Start,
            ct);
    }

    public async Task AddSessionAsync(Session session, CancellationToken ct)
    {
        await _db.Sessions.AddAsync(session, ct);
    }

    public async Task<List<Enrollment>> GetActiveEnrollmentsForSessionAsync(int sessionId, CancellationToken ct)
    {
        return await _db.Enrollments
            .AsNoTracking()
            .Include(e => e.Member)
            .Where(e => e.SessionId == sessionId && e.Status != EnrollmentStatus.Cancelled)
            .ToListAsync(ct);
    }

    public async Task<List<Session>> GetUpcomingSessionsByInstructorAsync(int instructorId, DateTime fromDate,
        CancellationToken ct)
    {
        return await _db.Sessions
            .AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.Place)
            .Include(s => s.Enrollments)
            .Where(s => s.InstructorId == instructorId && s.Start >= fromDate)
            .OrderBy(s => s.Start)
            .ToListAsync(ct);
    }

    public async Task<List<Class>> GetClassesAsync(CancellationToken ct)
    {
        return await _db.Classes
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }

    public async Task<List<Place>> GetPlacesAsync(CancellationToken ct)
    {
        return await _db.Places
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(ct);
    }

    public async Task<Session?> GetSessionByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Sessions.FindAsync(new object[] { id }, cancellationToken: ct);
    }

    public void DeleteSession(Session session)
    {
        _db.Sessions.Remove(session);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }
}