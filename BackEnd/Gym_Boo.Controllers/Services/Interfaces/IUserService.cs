using Gym_Boo.Data.Entities;

namespace Gym_Boo.Controllers.Services;

public interface IUserService
{
    Task<string?> RegisterMemberAsync(
        string name,
        string lastName,
        string email,
        string password);

    Task<User?> ValidateCredentialsAsync(
        string email,
        string password);
}