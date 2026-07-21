using Gym_Boo.ControllerApi.Dtos;
using Gym_Boo.Data.Entities;

namespace Gym_Boo.ControllerApi.Services;

public interface IReviewService
{
    Task<Review> CreateReviewAsync(string reviewTypeRaw, CreateReviewDto dto);
}