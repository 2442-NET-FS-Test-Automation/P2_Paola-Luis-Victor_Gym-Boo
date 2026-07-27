using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Controllers.Services;

public class InstructorServices : IInstructorServices
{
    private readonly GymBooDbContext _db;

    public InstructorServices(GymBooDbContext db)
    {
        _db = db;
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
            .Where(e => e.SessionId == id && e.Status == EnrollmentStatus.Enrolled)
            .Select(e => new SubscriberDto(
                e.MemberId,
                e.Member.Email
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
            .Where(s => s.InstructorId == instructorId && s.Start >= now)
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
}