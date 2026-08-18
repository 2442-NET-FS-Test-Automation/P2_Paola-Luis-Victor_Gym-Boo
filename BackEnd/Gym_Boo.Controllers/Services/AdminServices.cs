using Gym_Boo.Data.DTOs;
using Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories.Interfaces;

namespace Gym_Boo.Controllers.Services;

public class AdminServices : IAdminServices
{
    private readonly IAdminRepository _repo;

    public AdminServices(IAdminRepository repo)
    {
        _repo = repo;
    }

    // Disciplines
    public Task<List<Discipline>> GetAllDisciplines(CancellationToken ct)
    {
        return _repo.GetAllDisciplinesAsync(ct);
    }

    public async Task<bool> NewDisciplineAsync(string discipline, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(discipline)) return false;

        var normalizedName = discipline.Trim().ToLower();
        if (await _repo.DisciplineExistsByNameAsync(normalizedName, ct))
        {
            return false;
        }

        var entity = new Discipline { Name = normalizedName, Available = true };
        await _repo.AddDisciplineAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return true;
    }

    public Task<bool> DeleteDiscipline(string discipline, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(discipline)) return Task.FromResult(false);
        return _repo.DeleteDisciplineByNameAsync(discipline.Trim().ToLower(), ct);
    }

    public Task<bool> ToggleDiscipline(int id, CancellationToken ct)
    {
        return _repo.ToggleDisciplineAvailabilityAsync(id, ct);
    }

    public Task<bool> UpdateDiscipline(int id, string newName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(newName)) return Task.FromResult(false);
        return _repo.UpdateDisciplineNameAsync(id, newName.Trim().ToLower(), ct);
    }

    // Instructors
    public Task<List<User>> GetAllInstructors(CancellationToken ct)
    {
        return _repo.GetUsersByRoleAsync(Role.Instructor, ct);
    }

    public Task<bool> GetInstructor(int id, CancellationToken ct)
    {
        return _repo.UserExistsByIdAndRoleAsync(id, Role.Instructor, ct);
    }

    public async Task<bool> NewInstructor(User newInstructor, CancellationToken ct)
    {
        if (newInstructor == null || string.IsNullOrWhiteSpace(newInstructor.Email)) return false;

        if (await _repo.UserExistsByEmailAsync(newInstructor.Email, ct))
        {
            return false;
        }

        newInstructor.Role = Role.Instructor;
        await _repo.AddUserAsync(newInstructor, ct);
        await _repo.SaveChangesAsync(ct);

        return true;
    }

    public Task<bool> DeleteInstructor(int id, CancellationToken ct)
    {
        return _repo.DeleteUserByIdAndRoleAsync(id, Role.Instructor, ct);
    }

    public async Task<bool> UpdateInstructor(User instructor, CancellationToken ct)
    {
        if (instructor == null) return false;

        var target = await _repo.GetUserByIdAndRoleAsync(instructor.Id, Role.Instructor, ct);
        if (target == null) return false;

        target.Name = instructor.Name;
        target.LastName = instructor.LastName;
        target.Email = instructor.Email;

        await _repo.SaveChangesAsync(ct);
        return true;
    }

    // Revenue & Analytics
// Revenue & Analytics
    public async Task<List<MostRatedDto>> MostPopularClass(CancellationToken ct)
    {
        return await _repo.GetMostPopularClassesAsync(5, ct);
    }

    public async Task<List<DisciplineReportDto>> RegistrationReports(CancellationToken ct)
    {
        return await _repo.GetRegistrationReportsAsync(ct);
    }

    public async Task<double[]> TotalRevenue(CancellationToken ct)
    {
        var limitDate = DateTime.UtcNow.AddDays(-30);

        var cancellationRev = await _repo.GetCancellationRevenueAsync(limitDate, ct);
        var subscriptionRev = await _repo.GetSubscriptionRevenueAsync(limitDate, ct);
        var totalRev = cancellationRev + subscriptionRev;

        return new double[]
        {
            (double)cancellationRev,
            (double)subscriptionRev,
            (double)totalRev
        };
    }
}