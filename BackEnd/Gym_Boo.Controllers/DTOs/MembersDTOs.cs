namespace Gym_Boo.ControllerApi.DTOs;

public record PlanDto(
    int Id,
    string Name,
    decimal Price,
    string Recurrence
);

public record MemberReportDto(
    int Id,
    string Name,
    string LastName,
    string Email,
    string UserType,
    bool IsActive,
    int? IdSubscription,
    int? PlanId,
    DateTimeOffset? StartTime,
    DateTimeOffset? EndTime,
    int ClassesAttended,
    decimal AvgReviews
);