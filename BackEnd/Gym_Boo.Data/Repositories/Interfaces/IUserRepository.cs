using Gym_Boo.Data.Entities;

namespace Gym_Boo.Data.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string normalizedEmail);
    Task AddMemberAsync(Member member);
    Task<User?> GetByEmailAsync(string normalizedEmail);
}