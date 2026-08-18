using Gym_Boo.Data.Entities;

namespace Gym_Boo.Data.Repositories.Interfaces;

public interface IInstructorRepository
{
    Task<User?> GetUserByIdAsync(int id, CancellationToken ct);

    Task<bool> HasSessionOverlapAsync(int placeId, int instructorId, DateTime start, DateTime end,
        CancellationToken ct);

    Task AddSessionAsync(Session session, CancellationToken ct);
    Task<List<Enrollment>> GetActiveEnrollmentsForSessionAsync(int sessionId, CancellationToken ct);
    Task<List<Session>> GetUpcomingSessionsByInstructorAsync(int instructorId, DateTime fromDate, CancellationToken ct);
    Task<List<Class>> GetClassesAsync(CancellationToken ct);
    Task<List<Place>> GetPlacesAsync(CancellationToken ct);
    Task<Session?> GetSessionByIdAsync(int id, CancellationToken ct);
    void DeleteSession(Session session);
    Task SaveChangesAsync(CancellationToken ct = default);
}