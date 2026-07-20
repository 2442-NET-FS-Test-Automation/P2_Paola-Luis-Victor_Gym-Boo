namespace Gym_Boo.Data.DTOs;

public record CancelReservationResultDto(
    int EnrollmentId,
    string Status,
    bool HasPenalty,
    string Message
);