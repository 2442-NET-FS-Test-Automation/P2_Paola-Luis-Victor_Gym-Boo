namespace Gym_Boo.Controllers.Instructor;

public record SessionAttendanceResponseDto(int SessionId, int TotalEnrolled, List<SubscriberDto> Subscribers);
public record SubscriberDto(int Id, string Email);
public record NewSessionDto(DateTime StartTime, DateTime EndTime, int Slots, decimal CancellationFee, int ClassId, int InstructorId, int PlaceId);