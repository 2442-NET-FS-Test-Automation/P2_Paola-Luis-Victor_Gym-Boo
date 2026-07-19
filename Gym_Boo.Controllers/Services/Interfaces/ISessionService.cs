using GymBoo.Data.Entities;

namespace GymBoo.ControllerApi.Services;

public interface ISessionService
{
    Task<IReadOnlyList<Session>> AllAsync();
    Task<Session?> ByIdAsync(int id);
}


