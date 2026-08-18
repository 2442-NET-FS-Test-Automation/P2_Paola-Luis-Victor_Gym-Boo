namespace Gym_Boo.Controllers.DTOs;

public record RevenueReportDto(
    double CancellationRevenue,
    double SubscriptionRevenue,
    double TotalRevenue
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

public record UpdateInstructorDto(
    int Id,
    string Name,
    string LastName,
    string Email
);

public record CreateInstructorDto(
    string FirstName,
    string LastName,
    string Email,
    string Password
);

public record DisciplineDto(string Name);