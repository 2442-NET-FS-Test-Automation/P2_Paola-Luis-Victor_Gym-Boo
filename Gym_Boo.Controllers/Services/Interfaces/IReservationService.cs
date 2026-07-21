using Gym_Boo.Data.Entities;
using Gym_Boo.ControllerApi.Dtos;
using Gym_Boo.Data.DTOs;

namespace Gym_Boo.ControllerApi.Services;

public interface IReservationService
{
    Task<EnrolledDto> ReserveClassAsync(CreateReservationDto dto);

    Task<CancelReservationResultDto> CancelReservationAsync(int enrollmentId, int userId);
}