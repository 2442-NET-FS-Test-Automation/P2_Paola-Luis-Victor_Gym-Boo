using FluentAssertions;
using Gym_Boo.ControllerApi.Dtos;
using Gym_Boo.ControllerApi.Services;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Gym_Boo.Tests.Unit.MemberArea;

public class ClassReservationServiceTests
{
    private static GymBooDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<GymBooDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GymBooDbContext(options);
    }

    private static Member CreateMember(bool isActive = true, DateTime? expirationDate = null)
    {
        return new Member
        {
            Id = 1,
            Name = "Paola",
            LastName = "Felix",
            Email = "paola@test.com",
            Role = Role.Member,
            IsActive = isActive,
            MemberSubscription = new MemberSubscription
            {
                Id = 1,
                MemberId = 1,
                PlanId = 1,
                StartDate = DateTime.UtcNow.AddDays(-10),
                ExpirationDate = expirationDate ?? DateTime.UtcNow.AddDays(30),
                Plan = new SubscriptionPlan
                {
                    Id = 1,
                    Name = "Monthly",
                    Price = 39.99m,
                    Recurrence = Recurrence.Monthly
                }
            }
        };
    }

    private static Session CreateSession(int id = 1, int slots = 10, DateTime? start = null, DateTime? end = null)
    {
        var sessionStart = start ?? DateTime.UtcNow.AddDays(1);
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
            Slots = slots,
            CancellationFee = 15.00m,
            Enrollments = new List<Enrollment>(),
            Reviews = new List<Review>()
        };
    }

    [Fact]
    public async Task ReserveClassAsync_WhenValidInput_ReturnsCreatedReservation()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var member = CreateMember();
        var session = CreateSession();
        var dto = new CreateReservationDto(session.Id, member.Id);

        sessionRepo.Setup(x => x.GetByIdAsync(session.Id)).ReturnsAsync(session);
        memberRepo.Setup(x => x.GetByIdWithSubscriptionAsync(member.Id)).ReturnsAsync(member);
        enrollmentRepo.Setup(x => x.MemberHasConflictReservationAsync(member.Id, session.Start, session.End)).ReturnsAsync(false);

        Enrollment? created = null;
        enrollmentRepo.Setup(x => x.AddEnrollmentAsync(It.IsAny<Enrollment>()))
            .Callback<Enrollment>(e => created = e)
            .Returns(Task.CompletedTask);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var result = await service.ReserveClassAsync(dto);

        result.Should().NotBeNull();
        result.MemberId.Should().Be(member.Id);
        result.SessionId.Should().Be(session.Id);
        created.Should().NotBeNull();
        created!.Status.Should().Be(EnrollmentStatus.Enrolled);
    }

    [Fact]
    public async Task ReserveClassAsync_WhenClassDoesNotExist_ThrowsInvalidOperationException()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var member = CreateMember();
        var dto = new CreateReservationDto(999, member.Id);

        sessionRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Session?)null);
        memberRepo.Setup(x => x.GetByIdWithSubscriptionAsync(member.Id)).ReturnsAsync(member);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var act = async () => await service.ReserveClassAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Session not found.*");
    }

    [Fact]
    public async Task ReserveClassAsync_WhenMemberDoesNotExist_ThrowsInvalidOperationException()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var session = CreateSession();
        var dto = new CreateReservationDto(session.Id, 999);

        sessionRepo.Setup(x => x.GetByIdAsync(session.Id)).ReturnsAsync(session);
        memberRepo.Setup(x => x.GetByIdWithSubscriptionAsync(999)).ReturnsAsync((Member?)null);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var act = async () => await service.ReserveClassAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Member not found.*");
    }

    [Fact]
    public async Task ReserveClassAsync_WhenMemberIsInactive_ThrowsInvalidOperationException()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var member = CreateMember(isActive: false);
        var session = CreateSession();
        var dto = new CreateReservationDto(session.Id, member.Id);

        sessionRepo.Setup(x => x.GetByIdAsync(session.Id)).ReturnsAsync(session);
        memberRepo.Setup(x => x.GetByIdWithSubscriptionAsync(member.Id)).ReturnsAsync(member);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var act = async () => await service.ReserveClassAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*inactive*");
    }

    [Fact]
    public async Task ReserveClassAsync_WhenMemberSubscriptionIsExpired_ThrowsInvalidOperationException()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var member = CreateMember(expirationDate: DateTime.UtcNow.AddDays(-1));
        var session = CreateSession();
        var dto = new CreateReservationDto(session.Id, member.Id);

        sessionRepo.Setup(x => x.GetByIdAsync(session.Id)).ReturnsAsync(session);
        memberRepo.Setup(x => x.GetByIdWithSubscriptionAsync(member.Id)).ReturnsAsync(member);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var act = async () => await service.ReserveClassAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*active or valid subscription*");
    }

    [Fact]
    public async Task ReserveClassAsync_WhenCapacityIsFull_ThrowsInvalidOperationException()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var member = CreateMember();
        var session = CreateSession(slots: 1);
        session.Enrollments = new List<Enrollment>
        {
            new() { Status = EnrollmentStatus.Enrolled },
            new() { Status = EnrollmentStatus.Enrolled }
        };

        var dto = new CreateReservationDto(session.Id, member.Id);

        sessionRepo.Setup(x => x.GetByIdAsync(session.Id)).ReturnsAsync(session);
        memberRepo.Setup(x => x.GetByIdWithSubscriptionAsync(member.Id)).ReturnsAsync(member);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var act = async () => await service.ReserveClassAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*available slots*");
    }

    [Fact]
    public async Task ReserveClassAsync_WhenMemberAlreadyHasReservationForSameClass_ThrowsInvalidOperationException()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var member = CreateMember();
        var session = CreateSession();
        var dto = new CreateReservationDto(session.Id, member.Id);

        sessionRepo.Setup(x => x.GetByIdAsync(session.Id)).ReturnsAsync(session);
        memberRepo.Setup(x => x.GetByIdWithSubscriptionAsync(member.Id)).ReturnsAsync(member);
        enrollmentRepo.Setup(x => x.MemberHasConflictReservationAsync(member.Id, session.Start, session.End)).ReturnsAsync(true);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var act = async () => await service.ReserveClassAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already booked*");
    }

    [Fact]
    public async Task ReserveClassAsync_WhenScheduleOverlapExists_ThrowsInvalidOperationException()
    {
        var sessionRepo = new Mock<ISessionRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        var memberRepo = new Mock<IMemberRepository>();
        await using var db = CreateDbContext();

        var member = CreateMember();
        var session = CreateSession();
        var dto = new CreateReservationDto(session.Id, member.Id);

        sessionRepo.Setup(x => x.GetByIdAsync(session.Id)).ReturnsAsync(session);
        memberRepo.Setup(x => x.GetByIdWithSubscriptionAsync(member.Id)).ReturnsAsync(member);
        enrollmentRepo.Setup(x => x.MemberHasConflictReservationAsync(member.Id, session.Start, session.End)).ReturnsAsync(true);

        var service = new ReservationService(enrollmentRepo.Object, db, sessionRepo.Object, memberRepo.Object);

        var act = async () => await service.ReserveClassAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already booked*");
    }
}