using System.ComponentModel.DataAnnotations;

namespace Gym_Boo.ControllerApi.Dtos;

public record CreateReviewDto(
    [Required] int EnrollmentId,
    [Required] int SessionId,
    [Range(1, 5, ErrorMessage = "The rating must be between 1 and 5 stars.")] int Rating,
    [MaxLength(1000, ErrorMessage = "The comment cannot exceed 1000 characters.")] string Comment
);