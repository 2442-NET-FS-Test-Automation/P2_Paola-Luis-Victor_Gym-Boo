using Gym_Boo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Controllers.Administration;

public class AdminServices : IAdminServices
{
    private readonly GymBooDbContext _db;

    public AdminServices(GymBooDbContext dbContext)
    {
        _db = dbContext;
    }
    
    public async Task<bool> NewDisciplineAsync(string discipline)
    {
        if (string.IsNullOrWhiteSpace(discipline)) return false;

        var normalizedName = discipline.Trim().ToLower();
        if (await _db.Disciplines.AnyAsync(d => d.Name == normalizedName))
        {
            return false;
        }

        var newDiscipline = new Discipline { Name = normalizedName, Available = true };
        _db.Disciplines.Add(newDiscipline);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteDiscipline(string discipline)
    {
        if (string.IsNullOrWhiteSpace(discipline)) return false;

        var disciplineToDelete = await _db.Disciplines
            .FirstOrDefaultAsync(d => d.Name == discipline.Trim().ToLower());
        
        if (disciplineToDelete == null)
        {
            return false;
        }
        
        _db.Disciplines.Remove(disciplineToDelete);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DisableDiscipline(int id)
    {
        var target = await _db.Disciplines.FirstOrDefaultAsync(d => d.Id == id);
        
        if (target == null)
        {
            return false;
        }
        
        target.Available = !target.Available;
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateDiscipline(int id, string newName)
    {
        // Fixed: Changed signature to take an ID and the new name to apply
        var target = await _db.Disciplines.FirstOrDefaultAsync(d => d.Id == id);
        
        if (target == null || string.IsNullOrWhiteSpace(newName))
        {
            return false;
        }
        
        target.Name = newName.Trim().ToLower();
        await _db.SaveChangesAsync();

        return true;
    }

    public Task GetInstructor(int id)
    {
        throw new NotImplementedException();
    }
    
    public async Task<bool> NewInstructor(User newInstructor)
    {
        if (newInstructor == null) return false;

        var normalizedName = newInstructor.Name.Trim().ToLower();
        var normalizedLastName = newInstructor.LastName.Trim().ToLower();

        var instructorExist = await _db.Users.AnyAsync(i => 
            i.Name.Trim().ToLower() == normalizedName 
            && i.LastName.Trim().ToLower() == normalizedLastName
            && i.Email == newInstructor.Email);

        if (!instructorExist)
        {
            _db.Users.Add(newInstructor);
            await _db.SaveChangesAsync();
            return true;
        }
        
        return false;
    }

    public async Task<bool> DeleteInstructor(int id)
    {
        var instructorToDelete = await _db.Users.FirstOrDefaultAsync(i => i.Id == id);
        
        if (instructorToDelete == null)
        {
            return false;
        }
        
        _db.Users.Remove(instructorToDelete);
        await _db.SaveChangesAsync();
    
        return true;
    }

    public async Task<bool> UpdateInstructor(User instructor)
    {
        // Implemented basic update logic stub assuming entity tracking
        if (instructor == null) return false;
        
        _db.Users.Update(instructor);
        await _db.SaveChangesAsync();
        return true;
    }
    
    public Task MostPopularClass()
    {
        throw new NotImplementedException();
    }

    public Task ReseravtionReports()
    {
        throw new NotImplementedException();
    }

    public Task ClassRevenue()
    {
        throw new NotImplementedException();
    }
}