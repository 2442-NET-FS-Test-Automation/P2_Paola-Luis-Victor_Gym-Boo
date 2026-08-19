using Gym_Boo.Data.DTOs;
using Gym_Boo.Data.Entities;

namespace Gym_Boo.Controllers.Services.Interfaces;

public interface IAdminServices
{
// Disciplines
    Task<List<Discipline>> GetAllDisciplines(CancellationToken ct);
    Task<Discipline> NewDisciplineAsync(string discipline, CancellationToken ct);
    Task UpdateDiscipline(int id, string newName, CancellationToken ct);
    Task ToggleDiscipline(int id, CancellationToken ct);
    Task DeleteDiscipline(string discipline, CancellationToken ct);

    // Instructors
    Task<List<User>> GetAllInstructors(CancellationToken ct);
    Task<User> GetInstructorById(int id, CancellationToken ct);
    Task<User> NewInstructor(CreateInstructorDto dto, CancellationToken ct);
    Task UpdateInstructor(UpdateInstructorDto dto, CancellationToken ct);
    Task DeleteInstructor(int id, CancellationToken ct);

    // Reports & Analytics
    Task<List<MostRatedDto>> MostPopularClass(CancellationToken ct);
    Task<List<DisciplineReportDto>> RegistrationReports(CancellationToken ct);
    Task<RevenueReportDto> TotalRevenue(CancellationToken ct);
}