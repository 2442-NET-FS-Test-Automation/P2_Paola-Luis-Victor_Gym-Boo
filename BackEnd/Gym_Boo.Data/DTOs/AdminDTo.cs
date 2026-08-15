namespace Gym_Boo.Data.DTOs;

public record MostRatedDto(
    int SessionId,
    string ClassName,
    string InstructorName,
    double AverageRating
);

public record DisciplineReportDto(
    string DisciplineName,
    int TotalEnrollments
);