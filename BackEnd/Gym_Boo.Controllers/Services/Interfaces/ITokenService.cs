using Gym_Boo.Data.Entities;

namespace Gym_Boo.Controllers.Services;

public interface ITokenService
{
    string Issue(User user);
}