using Gym_Boo.ControllerApi.Dtos;
using Gym_Boo.ControllerApi.Services;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Repositories;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.DTOs;

namespace Gym_Boo.ControllerApi.Services;

public class ReservationService : IReservationService
{
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly ISessionRepository _sessionRepo;
    private readonly IMemberRepository _memberRepo;
    private readonly GymBooDbContext _context;

    public ReservationService(IEnrollmentRepository enrollmentRepo, GymBooDbContext context, ISessionRepository sessionRepo, IMemberRepository memberRepo)
    {
        _enrollmentRepo = enrollmentRepo;
        _context = context;
        _sessionRepo = sessionRepo;
        _memberRepo = memberRepo;
    }

    public async Task<CancelReservationResultDto> CancelReservationAsync(int enrollmentId, int userId)
    {
        // 1. Obtain reservation data with its session
        var enrollment = await _enrollmentRepo.GetByIdWithSessionAsync(enrollmentId);

        if (enrollment is null || enrollment.MemberId != userId)
        {
            throw new KeyNotFoundException("The requested reservation for this user was not found.");
        }

        if (enrollment.Status == EnrollmentStatus.Cancelled)
        {
            throw new InvalidOperationException("This reservation has already been cancelled.");
        }

        // 2. Calculates the difference between current time and sessionStart
        var now = DateTime.UtcNow;
        var timeUntilSession = enrollment.Session.Start - now;

        bool hasPenalty = false;
        decimal penaltyAmount = 0M;
        enrollment.Status = EnrollmentStatus.Cancelled;

        if (timeUntilSession.TotalHours < 0)
        {
            throw new InvalidOperationException("This reservation has already expired.");
        }
        
        // 3. Business rule: less than 2 hours for starting session
        if (timeUntilSession.TotalHours < 2)
        {
            enrollment.CancellationFeeApplied = true;
            penaltyAmount = enrollment.Session.CancellationFee;
            hasPenalty = true;
        }

        // 4. Guardar cambios (libera automáticamente el cupo en la consulta de disponibles)
        await _enrollmentRepo.UpdateAsync(enrollment);
        await _context.SaveChangesAsync();

        return new CancelReservationResultDto(
            EnrollmentId: enrollment.Id,
            Status: hasPenalty ? "Cancelled with penalty" : "Free cancellation",
            HasPenalty: hasPenalty,
            Amount: penaltyAmount
        );
    }

    public async Task<UserReservationsResponseDto> GetUserReservationHistoryAsync(int userId)
    {
        var enrollments = await _enrollmentRepo.GetByUserIdAsync(userId);
        var now = DateTime.UtcNow;

        var mappedItems = enrollments.Select(e => new ReservationItemDto(
            EnrollmentId: e.Id,
            SessionId: e.SessionId,
            ClassName: e.Session?.Class?.Name ?? "Class without a name",
            Discipline: e.Session?.Class?.Discipline.Name ?? "General",
            InstructorName: e.Session?.Instructor?.Name ?? "Without instructor",
            StartTime: e.Session?.Start ?? DateTime.MinValue,
            EndTime: e.Session?.End ?? DateTime.MinValue,
            Location: e.Session?.Place?.Name ?? "Location not specified",
            Status: e.Status.ToString(),
            HasPenalty: e.CancellationFeeApplied
        )).ToList();

        // Classify Past and On Next
        var upcoming = mappedItems
            .Where(item => item.StartTime >= now)
            .OrderBy(item => item.StartTime) // Next in ascending order (closest first)
            .ToList();

        var past = mappedItems
            .Where(item => item.StartTime < now)
            .OrderByDescending(item => item.StartTime) // Passes in descending order (most recent first)
            .ToList();

        return new UserReservationsResponseDto(Upcoming: upcoming, Past: past);
    }

    public async Task<EnrolledDto> ReserveClassAsync(CreateReservationDto dto)
    {
        // 1. Validates existent class session
        var session = await _sessionRepo.GetByIdAsync(dto.SessionId);
        if (session is null)
        {
            throw new InvalidOperationException("Session not found.");
        }

        // 2. Validate member enabled and with valid subscription
        var member = await _memberRepo.GetByIdWithSubscriptionAsync(dto.MemberId);
        if (member is null)
        {
            throw new InvalidOperationException("Member not found.");
        }

        // Verificación 1: Que el miembro esté activo
        if (!member.IsActive)
        {
            throw new InvalidOperationException("Member account is inactive.");
        }

        // Verificación 2: Que tenga suscripción y no haya expirado
        var currentSubscription = member.MemberSubscription; // O FirstOrDefault / ActiveSubscription si es colección
        if (currentSubscription is null || currentSubscription.ExpirationDate < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Member does not have an active or valid subscription.");
        }


        // Criterio de Aceptación: Validar Capacidad Disponible
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

        
        return new EnrolledDto(enrollment.Id, enrollment.MemberId, enrollment.SessionId);
    }
}