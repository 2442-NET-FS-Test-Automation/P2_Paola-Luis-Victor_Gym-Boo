using Gym_Boo.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Controllers.Instructor;

public class InstructorServices : IInstructorServices
{
    private readonly GymBooDbContext _db;
    
    public InstructorServices(GymBooDbContext db)
    {
        _db = db;
    }
    
    public async Task<User> GetInstructor(string email, string hashed)
    { 
        var ans = await _db.Users.FirstOrDefaultAsync(i => i.Email == email && i.PasswordHash == hashed);
        if (ans == null)
        {
            return null;
        }
        return ans;
    }

    public async Task<bool> NewSession(Session session, CancellationToken ct)
    {
        var check = await _db.Sessions.FirstOrDefaultAsync(i => i.Id == session.Id);
        if (check != null)
            return false;
        
        await _db.Sessions.AddAsync(session);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public Task<List<User>> Registered(int id)
    {
        throw new NotImplementedException();
    }
}