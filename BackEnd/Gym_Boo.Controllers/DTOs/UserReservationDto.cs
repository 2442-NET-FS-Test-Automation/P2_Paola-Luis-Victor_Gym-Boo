namespace Gym_Boo.ControllerApi.Dtos;

public record ReservationItemDto(
    int EnrollmentId,
    int SessionId,
    string ClassName,
    string Discipline,
    string InstructorName,
    DateTime StartTime,
    DateTime EndTime,
    string Location,
    string Status, // Enrolled, Cancelled
    bool HasPenalty
);

public record UserReservationsResponseDto(
    IReadOnlyList<ReservationItemDto> Upcoming,
    IReadOnlyList<ReservationItemDto> Past
);