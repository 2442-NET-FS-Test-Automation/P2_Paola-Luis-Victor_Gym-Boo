namespace Gym_Boo.ControllerApi.Dtos;

public record ReservationItemDto(
    int EnrollmentId,
    int SessionId,
    string ClassName,
    string Discipline,
    string InstructorName,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string Location,
    string Status, // Enrolled, Cancelled
    bool HasPenalty,
    decimal Penalty
);

public record UserReservationsResponseDto(
    IReadOnlyList<ReservationItemDto> Upcoming,
    IReadOnlyList<ReservationItemDto> Past
);