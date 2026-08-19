using Gym_Boo.Data.DTOs;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;

namespace Gym_Boo.Data.Repositories.Interfaces;

public interface IAdminRepository
{
    // Discipline Operations
    Task<List<Discipline>> GetAllDisciplinesAsync(CancellationToken ct);
    Task<bool> DisciplineExistsByNameAsync(string name, CancellationToken ct);
    Task AddDisciplineAsync(Discipline discipline, CancellationToken ct);
    Task<bool> DeleteDisciplineByNameAsync(string name, CancellationToken ct);
    Task<bool> ToggleDisciplineAvailabilityAsync(int id, CancellationToken ct);
    Task<bool> UpdateDisciplineNameAsync(int id, string newName, CancellationToken ct);

    // Instructor / User Operations
    Task<List<User>> GetUsersByRoleAsync(Role role, CancellationToken ct);
    Task<bool> UserExistsByIdAndRoleAsync(int id, Role role, CancellationToken ct);
    Task<bool> UserExistsByEmailAsync(string email, CancellationToken ct);
    Task AddUserAsync(User user, CancellationToken ct);
    Task<bool> DeleteUserByIdAndRoleAsync(int id, Role role, CancellationToken ct);
    Task<User?> GetUserByIdAndRoleAsync(int id, Role role, CancellationToken ct);

    // Analytics & Revenue Operations
    Task<List<MostRatedDto>> GetMostPopularClassesAsync(int limit, CancellationToken ct);
    Task<List<DisciplineReportDto>> GetRegistrationReportsAsync(CancellationToken ct);
    Task<decimal> GetCancellationRevenueAsync(DateTime sinceDate, CancellationToken ct);
    Task<decimal> GetSubscriptionRevenueAsync(DateTime sinceDate, CancellationToken ct);

    // Persistence
    Task<int> SaveChangesAsync(CancellationToken ct);
}