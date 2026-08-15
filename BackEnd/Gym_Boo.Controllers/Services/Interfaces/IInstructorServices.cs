using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Data.Entities;
using GymBoo.ControllerApi.DTOs;

namespace Gym_Boo.Controllers.Services.Interfaces;

public interface IInstructorServices
{
    Task<User?> GetInstructor(int id, CancellationToken ct);
    Task<bool> NewSession(Session session, CancellationToken ct);
    Task<SessionAttendanceResponseDto> GetAttendance(int id, CancellationToken ct);
    Task<List<UpcomingSessionDto>> GetUpcomingSessionsForInstructor(int instructorId, CancellationToken ct);
    Task<List<ClassOptionDto>> GetClassOptions(CancellationToken ct);
    Task<List<PlaceOptionDto>> GetPlaceOptions(CancellationToken ct);
    Task<bool> DeleteSession(int id, CancellationToken ct);
    Task<bool> TakeAttendance(TakingAttendanceDTO dto, CancellationToken ct = default);
}