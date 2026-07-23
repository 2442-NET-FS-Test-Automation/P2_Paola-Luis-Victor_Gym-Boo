using Gym_Boo.Data.DTOs;
using Gym_Boo.Data.Entities;

namespace Gym_Boo.ControllerApi.Services;

public interface ISessionService
{
    Task<IReadOnlyList<Session>> AllAsync();
    Task<Session?> ByIdAsync(int id);
    Task<IReadOnlyList<ClassSessionDto>> GetFilteredSessionsAsync(string? discipline, DateTime? date, bool past);
}


