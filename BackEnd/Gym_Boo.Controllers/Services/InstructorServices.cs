using System.ComponentModel.DataAnnotations;
using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories;
using Gym_Boo.Data.Repositories.Interfaces;
using GymBoo.ControllerApi.DTOs;
using Microsoft.EntityFrameworkCore;

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

    public async Task<bool> NewSession(Session session, CancellationToken ct)
    {
        if (session.Start >= session.End) return false;

        bool isOccupied = await _instructorRepository.HasSessionOverlapAsync(
            session.PlaceId,
            session.InstructorId,
            session.Start,
            session.End,
            ct);

        if (isOccupied) return false;

        try
        {
            await _instructorRepository.AddSessionAsync(session, ct);
            await _instructorRepository.SaveChangesAsync(ct);
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
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
        var sessions =
            await _instructorRepository.GetUpcomingSessionsByInstructorAsync(instructorId, DateTime.UtcNow, ct);

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

    public async Task<bool> DeleteSession(int id, CancellationToken ct)
    {
        try
        {
            var session = await _instructorRepository.GetSessionByIdAsync(id, ct);
            if (session == null) return false;

            _instructorRepository.DeleteSession(session);
            await _instructorRepository.SaveChangesAsync(ct);
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task<bool> TakeAttendance(TakingAttendanceDTO dto, CancellationToken ct = default)
    {
        if (dto == null) return false;

        var enrollment = await _enrollmentRepository.GetByIdWithSessionAsync(dto.EnrollmentId)
                         ?? throw new ArgumentException("Invalid enrollment Id");

        Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);

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
            return false;
        }

        await _enrollmentRepository.UpdateAsync(enrollment);
        await _instructorRepository.SaveChangesAsync(ct);

        return true;
    }
}