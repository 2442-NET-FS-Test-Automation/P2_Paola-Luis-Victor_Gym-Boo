using Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Data.DTOs;
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

    public async Task<Discipline> NewDisciplineAsync(string discipline, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(discipline))
            throw new ArgumentException("Discipline name cannot be empty.", nameof(discipline));

        var normalizedName = discipline.Trim().ToLower();
        if (await _repo.DisciplineExistsByNameAsync(normalizedName, ct))
            throw new InvalidOperationException($"Discipline '{normalizedName}' already exists.");

        var entity = new Discipline { Name = normalizedName, Available = true };
        await _repo.AddDisciplineAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return entity;
    }

    public async Task UpdateDiscipline(int id, string newName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("New discipline name cannot be empty.", nameof(newName));

        var success = await _repo.UpdateDisciplineNameAsync(id, newName.Trim().ToLower(), ct);
        if (!success)
            throw new KeyNotFoundException($"Discipline with ID {id} was not found.");
    }

    public async Task ToggleDiscipline(int id, CancellationToken ct)
    {
        var success = await _repo.ToggleDisciplineAvailabilityAsync(id, ct);
        if (!success)
            throw new KeyNotFoundException($"Discipline with ID {id} was not found.");
    }

    public async Task DeleteDiscipline(string discipline, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(discipline))
            throw new ArgumentException("Discipline name cannot be empty.", nameof(discipline));

        var success = await _repo.DeleteDisciplineByNameAsync(discipline.Trim().ToLower(), ct);
        if (!success)
            throw new KeyNotFoundException($"Discipline '{discipline}' was not found.");
    }

    // Instructors
    public Task<List<User>> GetAllInstructors(CancellationToken ct)
    {
        return _repo.GetUsersByRoleAsync(Role.Instructor, ct);
    }

    public async Task<User> GetInstructorById(int id, CancellationToken ct)
    {
        var instructor = await _repo.GetUserByIdAndRoleAsync(id, Role.Instructor, ct);
        if (instructor == null)
            throw new KeyNotFoundException($"Instructor with ID {id} was not found.");

        return instructor;
    }

    public async Task<User> NewInstructor(CreateInstructorDto dto, CancellationToken ct)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("Instructor details and valid email are required.");

        if (await _repo.UserExistsByEmailAsync(dto.Email, ct))
            throw new InvalidOperationException($"A user with email '{dto.Email}' already exists.");

        var instructor = new User
        {
            Email = dto.Email,
            Name = dto.FirstName,
            LastName = dto.LastName,
            Role = Role.Instructor,
            IsActive = true
        };

        await _repo.AddUserAsync(instructor, ct);
        await _repo.SaveChangesAsync(ct);

        return instructor;
    }

    public async Task UpdateInstructor(UpdateInstructorDto dto, CancellationToken ct)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var target = await _repo.GetUserByIdAndRoleAsync(dto.Id, Role.Instructor, ct);
        if (target == null)
            throw new KeyNotFoundException($"Instructor with ID {dto.Id} was not found.");

        target.Name = dto.Name;
        target.LastName = dto.LastName;
        target.Email = dto.Email;

        await _repo.SaveChangesAsync(ct);
    }

    public async Task DeleteInstructor(int id, CancellationToken ct)
    {
        var success = await _repo.DeleteUserByIdAndRoleAsync(id, Role.Instructor, ct);
        if (!success)
            throw new KeyNotFoundException($"Instructor with ID {id} was not found.");
    }

    // Reports & Analytics
    public Task<List<MostRatedDto>> MostPopularClass(CancellationToken ct)
    {
        return _repo.GetMostPopularClassesAsync(5, ct);
    }

    public Task<List<DisciplineReportDto>> RegistrationReports(CancellationToken ct)
    {
        return _repo.GetRegistrationReportsAsync(ct);
    }

    public async Task<RevenueReportDto> TotalRevenue(CancellationToken ct)
    {
        var limitDate = DateTime.UtcNow.AddDays(-30);

        var cancellationRevenue = await _repo.GetCancellationRevenueAsync(limitDate, ct);
        var subscriptionRevenue = await _repo.GetSubscriptionRevenueAsync(limitDate, ct);
        var totalRevenue = cancellationRevenue + subscriptionRevenue;

        return new RevenueReportDto(cancellationRevenue, subscriptionRevenue, totalRevenue);
    }
}