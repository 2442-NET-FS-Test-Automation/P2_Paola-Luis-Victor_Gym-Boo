namespace Gym_Boo.ControllerApi.Dtos;

public record CancelReservationResultDto(
    int EnrollmentId,
    string Status,
    bool HasPenalty,
    decimal Amount
);