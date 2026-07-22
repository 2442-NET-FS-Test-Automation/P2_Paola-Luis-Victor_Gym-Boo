using Gym_Boo.Data.Entities;

namespace Gym_Boo.Controllers.Instructor;

public interface IInstructorServices
{
    //Instructor profile
    Task<User> GetInstructor(string email, string password);
    
    //Session Manaagement
    Task<bool> NewSession(Session session, CancellationToken ct);

    Task<List<User>> Registered(int id);
}