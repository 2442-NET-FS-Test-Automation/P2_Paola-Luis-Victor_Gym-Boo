namespace Gym_Boo.Data.DTOs;

public record ClassSessionDto(
    int Id,
    string ClassName,
    string Discipline,
    string InstructorName,
    decimal InstructorRating,
    DateTime StartTime,
    DateTime EndTime,
    string Location,
    int AvailableSpots,
    int TotalSpots
);