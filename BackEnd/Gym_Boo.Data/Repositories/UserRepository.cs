using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GymBooDbContext _db;

    public UserRepository(GymBooDbContext db)
    {
        _db = db;
    }

    public async Task<bool> EmailExistsAsync(string normalizedEmail)
    {
        return await _db.Users.AnyAsync(user => user.Email == normalizedEmail);
    }

    public async Task AddMemberAsync(Member member)
    {
        _db.Members.Add(member);
        await _db.SaveChangesAsync();
    }

    public async Task<User?> GetByEmailAsync(string normalizedEmail)
    {
        return await _db.Users.SingleOrDefaultAsync(user => user.Email == normalizedEmail);
    }
}