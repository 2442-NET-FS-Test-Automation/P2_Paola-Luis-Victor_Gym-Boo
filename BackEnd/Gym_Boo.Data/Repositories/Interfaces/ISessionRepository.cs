using Gym_Boo.Data.Entities;

namespace Gym_Boo.Data.Repositories;

public interface ISessionRepository
{

    Task<IReadOnlyList<Session>> GetAllAsync();
    Task<IReadOnlyList<Session>> GetAllOnNextAsync();
    Task<Session?> GetByIdAsync(int id);
    Task AddAsync(Session session);
    Task UpdateAsync(Session session);
    Task<bool> DeleteAsync(int id);
    Task<IReadOnlyList<Session>> GetAvailableClassesAsync(string? discipline, DateTime? date, bool past);
}