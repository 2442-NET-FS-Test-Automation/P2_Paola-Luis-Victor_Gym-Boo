using Gym_Boo.Data.Entities;

namespace Gym_Boo.Controllers.Instructor;

public interface IInstructorServices
{
    //Instructor profile
    Task<User> GetInstructor(int id, CancellationToken ct);
    
    //Session Manaagement
    Task<bool> NewSession(Session session, CancellationToken ct);
    
    //Class
    Task<SessionAttendanceResponseDto> GetAttendance(int id, CancellationToken ct);
}