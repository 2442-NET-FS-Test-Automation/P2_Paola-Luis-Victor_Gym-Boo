using Gym_Boo.Data.Entities;

namespace Gym_Boo.Data.Repositories;

public interface IEnrollmentRepository
{
    Task<bool> MemberHasConflictReservationAsync(int userId, DateTime startTime, DateTime endTime);

    Task AddEnrollmentAsync(Enrollment enrollment);

    Task<Enrollment?> GetByIdWithSessionAsync(int enrollmentId);

    Task UpdateAsync(Enrollment enrollment);

    Task<IReadOnlyList<Enrollment>> GetByUserIdAsync(int userId);

    Task<bool> EnrollmentHasBeenAttendedAsync(int enrollmentId);
}