namespace Gym_Boo.Controllers.Administration;

public interface IAdminServices
{
    //Discipline related functions
    Task newDiscipline(string discipline);
    Task deleteDiscipline(string discipline);
    Task updateDiscipline(string discipline);
    
    //Instructor related functions
    Task newInstructor(string instructor);
    Task deleteInstructor(string instructor);
    Task updateInstructor(string instructor);
    
    //Revenue Related functions
    Task MostPopularClass();
    Task ReseravtionReports();
    Task ClassRevenue();
    
}