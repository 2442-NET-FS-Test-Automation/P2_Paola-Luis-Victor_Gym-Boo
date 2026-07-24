using Gym_Boo.Data.Enums;

namespace Gym_Boo.ControllerApi.Dtos;

public record ReviewDto(
    int ReviewId,
    int EnrollmentId,
    ReviewType ReviewType,
    int Rating,
    string? Comment,
    DateTimeOffset CreatedAt
);