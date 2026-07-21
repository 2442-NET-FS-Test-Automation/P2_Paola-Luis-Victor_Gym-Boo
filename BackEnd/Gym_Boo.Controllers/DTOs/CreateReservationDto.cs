namespace Gym_Boo.ControllerApi.Dtos;

public record CreateReservationDto(
    int SessionId,
    int MemberId
);