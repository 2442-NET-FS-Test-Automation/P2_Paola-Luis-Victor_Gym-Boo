using Gym_Boo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Data.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly GymBooDbContext _context;

    public SessionRepository(GymBooDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Session session)
    {
        await _context.Sessions.AddAsync(session);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var session = await _context.Sessions.FindAsync(id);

        if (session is null)
            return false;

        _context.Sessions.Remove(session);

        return true;
    }

    public async Task<IReadOnlyList<Session>> GetAllAsync()
    {
        return await _context.Sessions
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Session>> GetAllOnNextAsync()
    {
        return await SessionsWithClass()

            .AsNoTracking()
            .Where(s => s.Start > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Session>> GetAvailableClassesAsync(string? discipline, DateTime? date)
    {
        var query = _context.Sessions
        .Include(s => s.Class)
            .ThenInclude(c => c.Discipline)
        .Include(s => s.Instructor)
        .Include(s => s.Enrollments)
        .Include(s => s.Place)
        .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(discipline))
        {
            query = query.Where(s => s.Class.Discipline.Name.ToLower() == discipline.ToLower());
        }

        if (date.HasValue)
        {
            var targetDate = date.Value.Date;
            query = query.Where(s => s.Start.Date == targetDate);
        }

        return await query.ToListAsync();
    }

    public async Task<Session?> GetByIdAsync(int id)
    {
        return await _context.Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public Task UpdateAsync(Session session)
    {
        _context.Sessions.Update(session);
        return Task.CompletedTask;
    }


    private IQueryable<Session> SessionsWithClass()
    {
        return _context.Sessions.Include(s => s.Class).Include(s => s.Instructor);
    }
}