using GymBoo.Data.Entities;
using GymBoo.Data.Repositories;

namespace GymBoo.ControllerApi.Services;

class SessionService : ISessionService
{
    private readonly SessionRepository _repo;

    public SessionService(SessionRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<Session>> AllAsync()
    {
        return _repo.GetAllAsync();
    }

    public Task<Session?> ByIdAsync(int id)
    {
        return _repo.GetByIdAsync(id);
    }
}