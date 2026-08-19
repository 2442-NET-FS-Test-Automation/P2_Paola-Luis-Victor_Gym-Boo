using FluentAssertions;
using Gym_Boo.ControllerApi.Services;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Gym_Boo.Tests.Unit.MemberArea;

public class ReservationCancellationServiceTests
{
    private static GymBooDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<GymBooDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GymBooDbContext(options);
    }

    private static Session CreateSession(int id = 1, DateTime? start = null, DateTime? end = null)
    {
        var sessionStart = start ?? DateTime.UtcNow.AddHours(4);
        var sessionEnd = end ?? sessionStart.AddHours(1);

        var instructor = new Instructor
        {
            Id = 10,
            Name = "Luis",
            LastName = "Mora",
            Email = "luis@test.com",
            Role = Role.Instructor,
            IsActive = true
        };

        var discipline = new Discipline
        {
            Id = 5,
            Name = "Yoga",
            Available = true
        };

        var place = new Place
        {
            Id = 7,
            Name = "Main Studio",
            MaxCapacity = 20
        };

        var classEntity = new Class
        {
            Id = 3,
            Name = "Sunrise Flow",
            Description = "Morning yoga",
            Discipline = discipline,
            DisciplineId = discipline.Id
        };

        return new Session
        {
            Id = id,
            Class = classEntity,
            ClassId = classEntity.Id,
            Instructor = instructor,
            InstructorId = instructor.Id,
            Place = place,
            PlaceId = place.Id,
            Start = sessionStart,
            End = sessionEnd,
            Slots = 10,
            CancellationFee = 15.00m,
            Enrollments = new List<Enrollment>(),
            Reviews = new List<Review>()
        };
    }

    [Fact]
    public async Task CancelReservationAsync_WhenCancellationIsBeforeCutoff_ReturnsCancelledWithoutFee()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var enrollment = new Enrollment
        {
            Id = 1,
            MemberId = 2,
            SessionId = 10,
            Status = EnrollmentStatus.Enrolled,
            Session = CreateSession(10, DateTime.UtcNow.AddHours(3), DateTime.UtcNow.AddHours(4)),
            CancellationFeeApplied = false
        };

        enrollmentRepo.Setup(x => x.GetByIdWithSessionAsync(enrollment.Id)).ReturnsAsync(enrollment);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var result = await service.CancelReservationAsync(enrollment.Id, enrollment.MemberId);

        result.Status.Should().Be("Free cancellation");
        result.HasPenalty.Should().BeFalse();
        result.Amount.Should().Be(0m);
    }

    [Fact]
    public async Task CancelReservationAsync_WhenCancellationIsInsideCutoff_ReturnsCancelledWithFee()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var enrollment = new Enrollment
        {
            Id = 2,
            MemberId = 2,
            SessionId = 11,
            Status = EnrollmentStatus.Enrolled,
            Session = CreateSession(11, DateTime.UtcNow.AddMinutes(90), DateTime.UtcNow.AddMinutes(150)),
            CancellationFeeApplied = false
        };

        enrollmentRepo.Setup(x => x.GetByIdWithSessionAsync(enrollment.Id)).ReturnsAsync(enrollment);
        enrollmentRepo.Setup(x => x.UpdateAsync(enrollment)).Returns(Task.CompletedTask);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var result = await service.CancelReservationAsync(enrollment.Id, enrollment.MemberId);

        result.Status.Should().Be("Cancelled with penalty");
        result.HasPenalty.Should().BeTrue();
        result.Amount.Should().Be(15.00m);
    }

    [Fact]
    public async Task CancelReservationAsync_WhenReservationDoesNotExist_ThrowsKeyNotFoundException()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        enrollmentRepo.Setup(x => x.GetByIdWithSessionAsync(404)).ReturnsAsync((Enrollment?)null);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var act = async () => await service.CancelReservationAsync(404, 99);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CancelReservationAsync_WhenReservationBelongsToAnotherMember_ThrowsKeyNotFoundException()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var enrollment = new Enrollment
        {
            Id = 3,
            MemberId = 5,
            SessionId = 12,
            Status = EnrollmentStatus.Enrolled,
            Session = CreateSession(12, DateTime.UtcNow.AddHours(5), DateTime.UtcNow.AddHours(6))
        };

        enrollmentRepo.Setup(x => x.GetByIdWithSessionAsync(enrollment.Id)).ReturnsAsync(enrollment);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var act = async () => await service.CancelReservationAsync(enrollment.Id, 1);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CancelReservationAsync_WhenReservationAlreadyCancelled_ThrowsInvalidOperationException()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var enrollment = new Enrollment
        {
            Id = 4,
            MemberId = 2,
            SessionId = 13,
            Status = EnrollmentStatus.Cancelled,
            Session = CreateSession(13, DateTime.UtcNow.AddHours(5), DateTime.UtcNow.AddHours(6))
        };

        enrollmentRepo.Setup(x => x.GetByIdWithSessionAsync(enrollment.Id)).ReturnsAsync(enrollment);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var act = async () => await service.CancelReservationAsync(enrollment.Id, enrollment.MemberId);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already been cancelled*");
    }

    [Fact]
    public async Task CancelReservationAsync_WhenReservationIsInThePast_ThrowsInvalidOperationException()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var enrollment = new Enrollment
        {
            Id = 5,
            MemberId = 2,
            SessionId = 14,
            Status = EnrollmentStatus.Enrolled,
            Session = CreateSession(14, DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1))
        };

        enrollmentRepo.Setup(x => x.GetByIdWithSessionAsync(enrollment.Id)).ReturnsAsync(enrollment);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var act = async () => await service.CancelReservationAsync(enrollment.Id, enrollment.MemberId);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*expired*");
    }
}