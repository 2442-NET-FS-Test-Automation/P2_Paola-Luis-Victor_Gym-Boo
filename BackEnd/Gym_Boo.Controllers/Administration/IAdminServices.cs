using Gym_Boo.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Boo.Controllers.Administration;

public interface IAdminServices
{
    //Discipline related functions
    Task<bool> NewDisciplineAsync(string discipline, CancellationToken ct);
    Task<bool> DeleteDiscipline(string discipline, CancellationToken ct);
    Task<bool> DisableDiscipline(int id, CancellationToken ct);
    Task<bool> UpdateDiscipline(int id, string newName, CancellationToken ct);
    
    //Instructor related functions
    Task<bool> GetInstructor(int id, CancellationToken ct);
    Task<bool> NewInstructor(User newInstructor, CancellationToken ct);
    Task<bool> DeleteInstructor(int id, CancellationToken ct);
    Task<bool> UpdateInstructor(User instructor, CancellationToken ct);
    
    //Revenue Related functions
    Task<List<Session>> MostPopularClass(CancellationToken ct);
    Task ReseravtionReports(CancellationToken ct);
    Task ClassRevenue(CancellationToken ct);
    
    //Class
    Task<SessionAttendanceResponseDto> GetAttendance(int id, CancellationToken ct);
}