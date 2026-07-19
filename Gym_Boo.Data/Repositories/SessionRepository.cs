using GymBoo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymBoo.Data.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly GymBooDbContext _context;

    public SessionRepository(GymBooDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Session session)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<Session>> GetAllAsync()
    {
        return await _context.Sessions.ToListAsync();
    }

    public async Task<IReadOnlyList<Session>> GetAllOnNextAsync()
    {
        return await SessionsWithClass()
        .Where(s => s.Start > DateTime.UtcNow)
        .ToListAsync();
    }

    public Task<Session?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Session session)
    {
        throw new NotImplementedException();
    }


    private IQueryable<Session> SessionsWithClass()
    {
        return _context.Sessions.Include(s => s.Class);
    }
}