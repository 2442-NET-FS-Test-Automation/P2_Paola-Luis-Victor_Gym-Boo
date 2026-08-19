using Gym_Boo.Data.DTOs;
using Gym_Boo.Data.Entities;

namespace Gym_Boo.Controllers.Services.Interfaces;

public interface IAdminServices
{
    // Discipline Management
    Task<List<Discipline>> GetAllDisciplines(CancellationToken ct = default);
    Task<bool> NewDisciplineAsync(string discipline, CancellationToken ct = default);
    Task<bool> DeleteDiscipline(string discipline, CancellationToken ct = default);
    Task<bool> ToggleDiscipline(int id, CancellationToken ct = default);
    Task<bool> UpdateDiscipline(int id, string newName, CancellationToken ct = default);

    // Instructor Management
    Task<List<User>> GetAllInstructors(CancellationToken ct = default);
    Task<bool> GetInstructor(int id, CancellationToken ct = default);
    Task<bool> NewInstructor(User newInstructor, CancellationToken ct = default);
    Task<bool> DeleteInstructor(int id, CancellationToken ct = default);
    Task<bool> UpdateInstructor(User instructor, CancellationToken ct = default);

    // Analytics & Revenue
    Task<List<MostRatedDto>> MostPopularClass(CancellationToken ct = default);
    Task<List<DisciplineReportDto>> RegistrationReports(CancellationToken ct = default);
    Task<double[]> TotalRevenue(CancellationToken ct = default);
}