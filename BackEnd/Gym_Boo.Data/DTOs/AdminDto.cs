namespace Gym_Boo.Data.DTOs;
    
public record UpdateInstructorDto(
    int Id,
    string Name,
    string LastName,
    string Email,
    bool IsActive
);

public record CreateInstructorDto(
    string FirstName,
    string LastName,
    string Email,
    string Password
);

public record RevenueReportDto(
    decimal CancellationRevenue,
    decimal SubscriptionRevenue,
    decimal TotalRevenue
);

public record DisciplineReportDto(
    string DisciplineName,
    int TotalEnrollments
);

public record MostRatedDto(
    int SessionId,
    string ClassName,
    string InstructorName,
    double AverageRating
);
