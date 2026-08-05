using System.ComponentModel.DataAnnotations;
using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories;
using GymBoo.ControllerApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Controllers.Services;

public class InstructorServices : IInstructorServices
{
    private readonly GymBooDbContext _db;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public InstructorServices(GymBooDbContext db, IEnrollmentRepository enrollmentRepository)
    {
        _db = db;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<User?> GetInstructor(int id, CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<bool> NewSession(Session session, CancellationToken ct)
    {
        bool isPlaceOccupied = await _db.Sessions.AnyAsync(s => 
                s.InstructorId == session.InstructorId &&
                s.PlaceId == session.PlaceId && // Mismo lugar
                session.Start < s.End &&        // La nueva sesión empieza antes de que termine la existente
                session.End > s.Start,          // La nueva sesión termina después de que empiece la existente
            ct);

        if (isPlaceOccupied)
        {
            return false; 
        }
        
        try
        {
            _db.Sessions.Add(session);
            await _db.SaveChangesAsync(ct);
        
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task<SessionAttendanceResponseDto> GetAttendance(int id, CancellationToken ct)
    {
        var subscribers = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.SessionId == id && e.Status != EnrollmentStatus.Cancelled)
            .Select(e => new SubscriberDto(
                e.Id,
                e.Member.Email,
                e.Status.Equals(EnrollmentStatus.Attended)
            ))
            .ToListAsync(ct);

        return new SessionAttendanceResponseDto(
            SessionId: id,
            TotalEnrolled: subscribers.Count,
            Subscribers: subscribers
        );
    }
    
    public async Task<List<UpcomingSessionDto>> GetUpcomingSessionsForInstructor(int instructorId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var upcomingSessions = await _db.Sessions
            // Filtramos por el instructor y solo clases en el futuro
            .Where(s => s.InstructorId == instructorId)
            // Ordenamos para que la clase más pronta aparezca primero
            .OrderBy(s => s.Start) 
            .Select(s => new UpcomingSessionDto(
                s.Id,
                s.Class.Name, // Asumiendo que tu entidad Class tiene una propiedad Name
                s.Place.Name, // Propiedad Name de la entidad Place que compartiste antes
                s.Start,
                s.End,
                s.Slots - s.Enrollments.Count // Calculamos cuántos lugares quedan disponibles
            ))
            .ToListAsync(ct);

        return upcomingSessions;
    }


    public async Task<List<ClassOptionDto>> GetClassOptions(CancellationToken ct)
    {
        return await _db.Classes
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new ClassOptionDto(
                c.Id,
                c.Name
            ))
            .ToListAsync(ct);
    }

    public async Task<List<PlaceOptionDto>> GetPlaceOptions(CancellationToken ct)
    {
        return await _db.Places
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new PlaceOptionDto(
                p.Id,
                p.Name
            ))
            .ToListAsync(ct);
    }

    public async Task<bool> DeleteSession(int id, CancellationToken ct)
    {
        try
        {
            var session = await _db.Sessions.FindAsync(new object[] { id }, cancellationToken: ct);

            if (session == null)
            {
                return false;
            }

            _db.Sessions.Remove(session);
            await _db.SaveChangesAsync(ct);

            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task<bool> TakeAttendance(TakingAttendanceDTO dto)
    {
        var enrollment = await _enrollmentRepository.GetByIdWithSessionAsync

        (dto.EnrollmentId) ?? throw new ArgumentException("Invalid enrollment Id");

        var context = new ValidationContext(dto);

        Validator.ValidateObject(dto, context, validateAllProperties: true);
        
        var now = DateTime.UtcNow;
        var timeAfterSession = now - enrollment.Session.Start;
        if (timeAfterSession.TotalHours < 0)
        {
            throw new InvalidOperationException("You cannot check attendance for this session yet.");
        }

        if (dto.Action.Equals("attended"))
        {
            enrollment.Status = EnrollmentStatus.Attended;

            await _enrollmentRepository.UpdateAsync(enrollment);
            await _db.SaveChangesAsync();

            return true;
        }
        else if (dto.Action.Equals("not attended"))
        {
            enrollment.Status = EnrollmentStatus.Enrolled;

            await _enrollmentRepository.UpdateAsync(enrollment);
            await _db.SaveChangesAsync();

            return true;
        }

        return false;
    }
}
