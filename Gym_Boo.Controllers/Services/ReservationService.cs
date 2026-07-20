using Gym_Boo.ControllerApi.Dtos;
using Gym_Boo.ControllerApi.Services;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Repositories;
using Gym_Boo.Data.Enums;

namespace Gym_Boo.ControllerApi.Services;

public class ReservationService : IReservationService
{
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly ISessionRepository _sessionRepo;
    private readonly GymBooDbContext _context;

    public ReservationService(IEnrollmentRepository enrollmentRepo, GymBooDbContext context, ISessionRepository sessionRepo)
    {
        _enrollmentRepo = enrollmentRepo;
        _context = context;
        _sessionRepo = sessionRepo;
    }

    public async Task<Enrollment> ReserveClassAsync(CreateReservationDto dto)
    {
        // 1. Validates existent class session
        var session = await _sessionRepo.GetByIdAsync(dto.SessionId);
        if (session is null)
        {
            throw new InvalidOperationException("Session not found.");
        }

        // 2. Criterio de Aceptación: Validar Capacidad Disponible
        int occupiedSpots = session.Enrollments?.Count(e => e.Status == EnrollmentStatus.Enrolled) ?? 0;
        int availableSpots = session.Slots - occupiedSpots;

        if (availableSpots <= 0)
        {
            throw new InvalidOperationException("There is not available slots for this session.");
        }

        // 3. Criterio de Aceptación: Validar Reserva Duplicada / Solapamiento de horario
        bool hasConflict = await _enrollmentRepo.MemberHasConflictReservationAsync(dto.MemberId, session.Start, session.End);
        if (hasConflict)
        {
            throw new InvalidOperationException("Yo have already booked this or another session for this schedule.");
        }

        // 4. Crear la reserva (Enrollment)
        var enrollment = new Enrollment
        {
            SessionId = dto.SessionId,
            MemberId = dto.MemberId,
            Status = EnrollmentStatus.Enrolled,
            EnrollmentDateTime = DateTime.UtcNow
        };

        await _enrollmentRepo.AddEnrollmentAsync(enrollment);
        
        await _context.SaveChangesAsync();

        return enrollment;
    }
}