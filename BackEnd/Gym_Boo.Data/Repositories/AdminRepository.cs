using Gym_Boo.Data.DTOs;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Data.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly GymBooDbContext _db;

    public AdminRepository(GymBooDbContext dbContext)
    {
        _db = dbContext;
    }

    // ==========================================
    // DISCIPLINE QUERIES & COMMANDS
    // ==========================================

    public async Task<List<Discipline>> GetAllDisciplinesAsync(CancellationToken ct)
    {
        return await _db.Disciplines
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<bool> DisciplineExistsByNameAsync(string name, CancellationToken ct)
    {
        return await _db.Disciplines
            .AsNoTracking()
            .AnyAsync(d => d.Name == name, ct);
    }

    public async Task AddDisciplineAsync(Discipline discipline, CancellationToken ct)
    {
        await _db.Disciplines.AddAsync(discipline, ct);
    }

    public async Task<bool> DeleteDisciplineByNameAsync(string name, CancellationToken ct)
    {
        int rowsAffected = await _db.Disciplines
            .Where(d => d.Name == name)
            .ExecuteDeleteAsync(ct);

        return rowsAffected > 0;
    }

    public async Task<bool> ToggleDisciplineAvailabilityAsync(int id, CancellationToken ct)
    {
        int rowsAffected = await _db.Disciplines
            .Where(d => d.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(d => d.Available, d => !d.Available), ct);

        return rowsAffected > 0;
    }

    public async Task<bool> UpdateDisciplineNameAsync(int id, string newName, CancellationToken ct)
    {
        int rowsAffected = await _db.Disciplines
            .Where(d => d.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(d => d.Name, newName), ct);

        return rowsAffected > 0;
    }

    // ==========================================
    // INSTRUCTOR / USER QUERIES & COMMANDS
    // ==========================================

    public async Task<List<User>> GetUsersByRoleAsync(Role role, CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Role == role)
            .ToListAsync(ct);
    }

    public async Task<bool> UserExistsByIdAndRoleAsync(int id, Role role, CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == id && u.Role == role, ct);
    }

    public async Task<bool> UserExistsByEmailAsync(string email, CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, ct);
    }

    public async Task AddUserAsync(User user, CancellationToken ct)
    {
        await _db.Users.AddAsync(user, ct);
    }

    public async Task<bool> DeleteUserByIdAndRoleAsync(int id, Role role, CancellationToken ct)
    {
        int rowsAffected = await _db.Users
            .Where(u => u.Id == id && u.Role == role)
            .ExecuteDeleteAsync(ct);

        return rowsAffected > 0;
    }

    public async Task<User?> GetUserByIdAndRoleAsync(int id, Role role, CancellationToken ct)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.Role == role, ct);
    }

    
// ==========================================
// ANALYTICS & REPORTS
// ==========================================

    public async Task<List<MostRatedDto>> GetMostPopularClassesAsync(int limit, CancellationToken ct)
    {
        return await _db.Sessions
            .OrderByDescending(s => s.Reviews.Average(r => (double?)r.Rating) ?? 0.0)
            .Select(s => new MostRatedDto(
                s.Id,
                s.Class.Name,
                s.Instructor.Name,
                s.Reviews.Average(r => (double?)r.Rating) ?? 0.0
            )).Take(5)
            .ToListAsync(ct);
    }

    public async Task<List<DisciplineReportDto>> GetRegistrationReportsAsync(CancellationToken ct)
    {
         return await _db.Disciplines
            .OrderByDescending(d => d.Classes
                .SelectMany(c => c.Sessions)
                .SelectMany(s => s.Enrollments)
                .Count())
            .Select(d => new DisciplineReportDto(
                d.Name,
                d.Classes
                    .SelectMany(c => c.Sessions)
                    .SelectMany(s => s.Enrollments)
                    .Count()
            ))
            .ToListAsync(ct);
    }

    public async Task<decimal> GetCancellationRevenueAsync(DateTime sinceDate, CancellationToken ct)
    {
        return await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.CancellationFeeApplied && e.Session.Start >= sinceDate)
            .Select(e => e.Session.CancellationFee)
            .SumAsync(ct);
    }

    public async Task<decimal> GetSubscriptionRevenueAsync(DateTime sinceDate, CancellationToken ct)
    {
        return await _db.MemberSubscriptions
            .AsNoTracking()
            .Where(ms => ms.StartDate >= sinceDate)
            .Select(ms => ms.Plan.Price)
            .SumAsync(ct);
    }

    // ==========================================
    // UNIT OF WORK / SAVE
    // ==========================================

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await _db.SaveChangesAsync(ct);
    }
}