// REQ-6: Reservation History
using FluentAssertions;
using Gym_Boo.ControllerApi.Services;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Gym_Boo.Tests.Unit.MemberArea;

public class ReservationHistoryServiceTests
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
    public async Task GetUserReservationHistoryAsync_WhenMemberHasReservations_ReturnsUpcomingAndPastGroups()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var upcomingSession = CreateSession(21, DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(3));
        var pastSession = CreateSession(22, DateTime.UtcNow.AddHours(-3), DateTime.UtcNow.AddHours(-2));

        var upcomingEnrollment = new Enrollment
        {
            Id = 10,
            MemberId = 1,
            SessionId = upcomingSession.Id,
            Status = EnrollmentStatus.Enrolled,
            Session = upcomingSession
        };

        var pastEnrollment = new Enrollment
        {
            Id = 11,
            MemberId = 1,
            SessionId = pastSession.Id,
            Status = EnrollmentStatus.Cancelled,
            Session = pastSession
        };

        enrollmentRepo.Setup(x => x.GetByUserIdAsync(1))
            .ReturnsAsync(new List<Enrollment> { upcomingEnrollment, pastEnrollment });

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var result = await service.GetUserReservationHistoryAsync(1);

        result.Upcoming.Should().HaveCount(1);
        result.Past.Should().HaveCount(1);
        result.Upcoming[0].SessionId.Should().Be(upcomingSession.Id);
        result.Past[0].SessionId.Should().Be(pastSession.Id);
    }

    [Fact]
    public async Task GetUserReservationHistoryAsync_WhenMemberHasNoReservations_ReturnsEmptyGroups()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        enrollmentRepo.Setup(x => x.GetByUserIdAsync(77))
            .ReturnsAsync(new List<Enrollment>());

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var result = await service.GetUserReservationHistoryAsync(77);

        result.Upcoming.Should().BeEmpty();
        result.Past.Should().BeEmpty();
    }
}