using Gym_Boo.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Boo.Controllers.Administration;

public interface IAdminServices
{
    //Discipline related functions
    Task<bool> NewDisciplineAsync(string discipline);
    Task<bool> DeleteDiscipline(string discipline);
    Task<bool> DisableDiscipline(int id);
    Task<bool> UpdateDiscipline(int id, string newName);
    
    //Instructor related functions
    Task<bool> GetInstructor(int id);
    Task<bool> NewInstructor(User newInstructor);
    Task<bool> DeleteInstructor(int id);
    Task<bool> UpdateInstructor(User instructor);
    
    //Revenue Related functions
    Task<List<Session>> MostPopularClass();
    Task ReseravtionReports();
    Task ClassRevenue();
    
}