using Gym_Boo.Data.Entities;
using Gym_Boo.ControllerApi.Dtos;

namespace Gym_Boo.ControllerApi.Services;

public interface IReservationService
{
    Task<Enrollment> ReserveClassAsync(CreateReservationDto dto);
}