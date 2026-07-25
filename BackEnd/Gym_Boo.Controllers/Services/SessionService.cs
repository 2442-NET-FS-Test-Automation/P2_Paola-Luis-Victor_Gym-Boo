using Gym_Boo.Data.DTOs;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories;

namespace Gym_Boo.ControllerApi.Services;

class SessionService : ISessionService
{
    private readonly ISessionRepository _repo;

    public SessionService(ISessionRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<Session>> AllAsync()
    {
        return _repo.GetAllAsync();
    }

    public Task<Session?> ByIdAsync(int id)
    {
        return _repo.GetByIdAsync(id);
    }

    public async Task<IReadOnlyList<ClassSessionDto>> GetFilteredSessionsAsync(string? discipline, DateTime? date, bool past = false)
    {
        var sessions = await _repo.GetAvailableClassesAsync(discipline, date, past);

        // Mapeamos las entidades al DTO requerido
        return sessions.Select(s => new ClassSessionDto(
            Id: s.Id,
            ClassName: s.Class?.Name ?? "No name",
            Discipline: s.Class?.Discipline?.Name ?? "General",
            InstructorName: s.Instructor.Name + " " + s.Instructor.LastName,
            InstructorRating: s.Instructor.Sessions
                    .SelectMany(sess => sess.Reviews)
                    .Where(r => r.ReviewType == ReviewType.Instructor)
                    .Average(r => (decimal?)r.Rating) ?? 0.0m,
            StartTime: DateTime.SpecifyKind(s.Start, DateTimeKind.Utc),
            EndTime: DateTime.SpecifyKind(s.End,DateTimeKind.Utc),
            Location: s.Place.Name,
            AvailableSpots: s.Slots - s.Enrollments.Count(e => e.Status == EnrollmentStatus.Enrolled),
            TotalSpots: s.Slots
        )).ToList();
    }


    private static int OccupiedSlots(ICollection<Enrollment> enrollments)
    {
        return enrollments?.Count(e => e.Status == EnrollmentStatus.Enrolled) ?? 0;
    }

    private decimal GetInstructorRating(Instructor? instructor)
    {
        if (instructor?.Sessions is null || !instructor.Sessions.Any())
            return 0.0m;

        // Aplanamos todas las reseñas de todas las sesiones que ha dado este instructor
        var instructorReviews = instructor.Sessions
            .SelectMany(s => s.Reviews ?? Enumerable.Empty<Review>())
            .Where(r => r.ReviewType.Equals(ReviewType.Instructor));

        if (!instructorReviews.Any())
            return 0.0m;

        // Calculamos el promedio redondeado a 1 decimal
        return Math.Round((decimal)instructorReviews.Average(r => r.Rating), 1);
    }
}