using System.ComponentModel.DataAnnotations;
using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories;
using Gym_Boo.Data.Repositories.Interfaces;
using GymBoo.ControllerApi.DTOs;

namespace Gym_Boo.Controllers.Services;

public class InstructorServices : IInstructorServices
{
    private readonly IInstructorRepository _instructorRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public InstructorServices(
        IInstructorRepository instructorRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _instructorRepository = instructorRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<User?> GetInstructor(int id, CancellationToken ct)
    {
        return await _instructorRepository.GetUserByIdAsync(id, ct);
    }

    public async Task NewSession(Session session, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(session);

        if (session.Start >= session.End)
        {
            throw new ArgumentException("Session start time must be earlier than the end time.", nameof(session));
        }

        bool isOccupied = await _instructorRepository.HasSessionOverlapAsync(
            session.PlaceId,
            session.InstructorId,
            session.Start,
            session.End,
            ct);

        if (isOccupied)
        {
            throw new InvalidOperationException("The venue or instructor is already booked for this time period.");
        }

        await _instructorRepository.AddSessionAsync(session, ct);
        await _instructorRepository.SaveChangesAsync(ct);
    }

    public async Task<SessionAttendanceResponseDto> GetAttendance(int id, CancellationToken ct)
    {
        var enrollments = await _instructorRepository.GetActiveEnrollmentsForSessionAsync(id, ct);

        var subscribers = enrollments
            .Select(e => new SubscriberDto(
                e.Id,
                e.Member.Email,
                e.Status == EnrollmentStatus.Attended
            ))
            .ToList();

        return new SessionAttendanceResponseDto(
            SessionId: id,
            TotalEnrolled: subscribers.Count,
            Subscribers: subscribers
        );
    }

    public async Task<List<UpcomingSessionDto>> GetUpcomingSessionsForInstructor(int instructorId, CancellationToken ct)
    {
        var sessions = await _instructorRepository.GetUpcomingSessionsByInstructorAsync(instructorId, DateTime.UtcNow, ct);

        return sessions.Select(s => new UpcomingSessionDto(
            s.Id,
            s.Class.Name,
            s.Place.Name,
            s.Start,
            s.End,
            s.Slots - s.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled)
        )).ToList();
    }

    public async Task<List<ClassOptionDto>> GetClassOptions(CancellationToken ct)
    {
        var classes = await _instructorRepository.GetClassesAsync(ct);
        return classes.Select(c => new ClassOptionDto(c.Id, c.Name)).ToList();
    }

    public async Task<List<PlaceOptionDto>> GetPlaceOptions(CancellationToken ct)
    {
        var places = await _instructorRepository.GetPlacesAsync(ct);
        return places.Select(p => new PlaceOptionDto(p.Id, p.Name)).ToList();
    }

    public async Task DeleteSession(int id, CancellationToken ct)
    {
        var session = await _instructorRepository.GetSessionByIdAsync(id, ct)
                      ?? throw new KeyNotFoundException($"Session with ID {id} was not found.");

        _instructorRepository.DeleteSession(session);
        await _instructorRepository.SaveChangesAsync(ct);
    }

    public async Task TakeAttendance(TakingAttendanceDTO dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);

        var enrollment = await _enrollmentRepository.GetByIdWithSessionAsync(dto.EnrollmentId)
                         ?? throw new KeyNotFoundException($"Enrollment with ID {dto.EnrollmentId} was not found.");

        if (DateTime.UtcNow < enrollment.Session.Start)
        {
            throw new InvalidOperationException("You cannot check attendance for this session yet.");
        }

        if (string.Equals(dto.Action, "attended", StringComparison.OrdinalIgnoreCase))
        {
            enrollment.Status = EnrollmentStatus.Attended;
        }
        else if (string.Equals(dto.Action, "not attended", StringComparison.OrdinalIgnoreCase))
        {
            enrollment.Status = EnrollmentStatus.Enrolled;
        }
        else
        {
            throw new ArgumentException($"Invalid attendance action '{dto.Action}'. Supported values are 'attended' or 'not attended'.", nameof(dto));
        }

        await _enrollmentRepository.UpdateAsync(enrollment);
        await _instructorRepository.SaveChangesAsync(ct);
    }
}