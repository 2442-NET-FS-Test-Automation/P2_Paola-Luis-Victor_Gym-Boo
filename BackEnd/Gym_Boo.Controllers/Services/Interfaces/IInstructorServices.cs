using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Data.Entities;

namespace Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Controllers.DTOs;

public interface IInstructorServices
{
    //Instructor profile
    Task<User> GetInstructor(int id, CancellationToken ct);
    
    //Session Manaagement
    Task<bool> NewSession(Session session, CancellationToken ct);
    
    //Class
    Task<SessionAttendanceResponseDto> GetAttendance(int id, CancellationToken ct);
    
    Task<List<UpcomingSessionDto>> GetUpcomingSessionsForInstructor(int instructorId, CancellationToken ct);

    Task<List<ClassOptionDto>> GetClassOptions(CancellationToken ct);

    Task<List<PlaceOptionDto>> GetPlaceOptions(CancellationToken ct);
    
    Task<bool> DeleteSession(int id, CancellationToken ct);
    
}
