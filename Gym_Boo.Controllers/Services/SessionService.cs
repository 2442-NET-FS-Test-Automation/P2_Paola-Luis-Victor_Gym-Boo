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

    public async Task<IReadOnlyList<ClassSessionDto>> GetFilteredSessionsAsync(string? discipline, DateTime? date)
    {
        var sessions = await _repo.GetAvailableClassesAsync(discipline, date);

        // Mapeamos las entidades al DTO requerido
        return sessions.Select(s => new ClassSessionDto(
            Id: s.Id,
            ClassName: s.Class?.Name ?? "No name",
            Discipline: s.Class?.Discipline.Name ?? "General",
            InstructorName: s.Instructor.Name + " " + s.Instructor.LastName,
            StartTime: s.Start,
            EndTime: s.End,
            Location: s.Place.Name,

            AvailableSpots: s.Slots - OccupiedSlots(s.Enrollments),
            TotalSpots: s.Slots
        )).ToList();
    }


    private static int OccupiedSlots(ICollection<Enrollment> enrollments)
    {
        return enrollments?.Count(e => e.Status == EnrollmentStatus.Enrolled) ?? 0;
    }
}