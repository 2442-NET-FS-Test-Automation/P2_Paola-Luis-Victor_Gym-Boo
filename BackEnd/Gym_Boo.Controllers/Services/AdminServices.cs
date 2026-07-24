using Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Microsoft.EntityFrameworkCore;
//
namespace Gym_Boo.Controllers.Services;

public class AdminServices : IAdminServices
{
    private readonly GymBooDbContext _db;

    public AdminServices(GymBooDbContext dbContext)
    {
        _db = dbContext;
    }

    //Disciplines

    public async Task<List<Discipline>> GetAllDisciplines(CancellationToken ct)
    {
        var res = _db.Disciplines.ToListAsync(ct);
        if (!res.Result.Any())
        {
            return null;
        }

        return await res;
    }
    
    public async Task<bool> NewDisciplineAsync(string discipline, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(discipline)) return false;

        var normalizedName = discipline.Trim().ToLower();
        if (await _db.Disciplines.AnyAsync(d => d.Name == normalizedName, ct))
        {
            return false;
        }

        var newDiscipline = new Discipline { Name = normalizedName, Available = true };
        _db.Disciplines.Add(newDiscipline);
        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> DeleteDiscipline(string discipline, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(discipline)) return false;

        var normalizedName = discipline.Trim().ToLower();
        var disciplineToDelete = await _db.Disciplines
            .FirstOrDefaultAsync(d => d.Name == normalizedName, ct);

        if (disciplineToDelete == null)
        {
            return false;
        }

        _db.Disciplines.Remove(disciplineToDelete);
        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> ToggleDiscipline(int id, CancellationToken ct)
    {
        var target = await _db.Disciplines.FirstOrDefaultAsync(d => d.Id == id, ct);

        if (target == null)
        {
            return false;
        }
        
        target.Available = !target.Available; // Explicitly set to false for a disable method
        
        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> UpdateDiscipline(int id, string newName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(newName)) return false;

        var target = await _db.Disciplines.FirstOrDefaultAsync(d => d.Id == id, ct);

        if (target == null)
        {
            return false;
        }

        target.Name = newName.Trim().ToLower();
        await _db.SaveChangesAsync(ct);

        return true;
    }

    //Instructors

    public async Task<List<User>> GetAllInstructors(CancellationToken ct)
    {
        var res = _db.Users.Where(i => i.Role == Role.Instructor).ToListAsync(ct);
        if (!res.Result.Any())
        {
            return null;
        }

        return await res;
    }
    
    public async Task<bool> GetInstructor(int id, CancellationToken ct)
    {
        return await _db.Users.AnyAsync(i => i.Id == id, ct);
    }

    public async Task<bool> NewInstructor(User newInstructor, CancellationToken ct)
    {
        if (newInstructor == null) return false;

        var normalizedName = newInstructor.Name?.Trim().ToLower();
        var normalizedLastName = newInstructor.LastName?.Trim().ToLower();

        var instructorExist = await _db.Users.AnyAsync(i =>
            i.Name.ToLower() == normalizedName &&
            i.LastName.ToLower() == normalizedLastName &&
            i.Email == newInstructor.Email, ct);

        if (instructorExist)
        {
            return false;
        }

        _db.Users.Add(newInstructor);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteInstructor(int id, CancellationToken ct)
    {
        var instructorToDelete = await _db.Users.FirstOrDefaultAsync(i => i.Id == id, ct);

        if (instructorToDelete == null)
        {
            return false;
        }

        _db.Users.Remove(instructorToDelete);
        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> UpdateInstructor(User instructor, CancellationToken ct)
    {
        if (instructor == null) return false;

        var exists = await _db.Users.AnyAsync(i => i.Id == instructor.Id, ct);
        if (!exists) return false;

        _db.Users.Update(instructor);
        await _db.SaveChangesAsync(ct);
        return true;
    }
    
    //Analytics & Reports

    public async Task<List<Session>> MostPopularClass(CancellationToken ct)
    {
        // Safe EF Core translation for calculating average with fallbacks
        return await _db.Sessions
            .Include(s => s.Class)
            .Include(s => s.Instructor)
            .OrderByDescending(s => s.Reviews.Average(r => (double?)r.Rating) ?? 0.0)
            .ToListAsync(ct);
    }

    public Task ReseravtionReports(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task ClassRevenue(CancellationToken ct)
    {
        throw new NotImplementedException();
    }


}