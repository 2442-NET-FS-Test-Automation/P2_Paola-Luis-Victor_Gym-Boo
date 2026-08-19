using FluentAssertions;
using Gym_Boo.ControllerApi.Services;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Gym_Boo.Tests.Unit.MemberArea;

public class SessionServiceTests
{
    private static GymBooDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<GymBooDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GymBooDbContext(options);
    }

    [Fact]
    public async Task GetFilteredSessionsAsync_WhenNoFiltersProvided_ReturnsAllActiveSessions()
    {
        // Arrange
        var repoMock = new Mock<ISessionRepository>();
        await using var db = CreateDbContext();

        var instructor = new Instructor
        {
            Id = 1,
            Name = "Luis",
            LastName = "Mora",
            Email = "luis@test.com",
            Role = Role.Instructor,
            IsActive = true
        };

        var discipline = new Discipline
        {
            Id = 1,
            Name = "Yoga",
            Available = true
        };

        var place = new Place
        {
            Id = 1,
            Name = "Main Studio",
            MaxCapacity = 20
        };

        var classEntity = new Class
        {
            Id = 1,
            Name = "Sunrise Flow",
            Description = "Morning yoga",
            Discipline = discipline,
            DisciplineId = discipline.Id
        };

        var session = new Session
        {
            Id = 1,
            Class = classEntity,
            ClassId = classEntity.Id,
            Instructor = instructor,
            InstructorId = instructor.Id,
            Place = place,
            PlaceId = place.Id,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(1).AddHours(1),
            Slots = 10,
            Enrollments = new List<Enrollment>()
        };

        db.Instructors.Add(instructor);
        db.Disciplines.Add(discipline);
        db.Places.Add(place);
        db.Classes.Add(classEntity);
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        repoMock
            .Setup(x => x.GetAvailableClassesAsync(null, null, false))
            .ReturnsAsync(new List<Session> { session });

        var service = new SessionService(repoMock.Object, db);

        // Act
        var result = await service.GetFilteredSessionsAsync(null, null, false);

        // Assert
        result.Should().HaveCount(1);
        result[0].Discipline.Should().Be("Yoga");
        result[0].InstructorName.Should().Be("Luis Mora");
    }

    [Fact]
    public async Task GetFilteredSessionsAsync_WhenValidFiltersProvided_ReturnsMatchingSessions()
    {
        // Arrange
        var repoMock = new Mock<ISessionRepository>();
        await using var db = CreateDbContext();

        var instructor = new Instructor
        {
            Id = 2,
            Name = "Ana",
            LastName = "Ruiz",
            Email = "ana@test.com",
            Role = Role.Instructor,
            IsActive = true
        };

        var discipline = new Discipline
        {
            Id = 2,
            Name = "Pilates",
            Available = true
        };

        var place = new Place
        {
            Id = 2,
            Name = "Pilates Room",
            MaxCapacity = 15
        };

        var classEntity = new Class
        {
            Id = 2,
            Name = "Core Strength",
            Description = "Pilates",
            Discipline = discipline,
            DisciplineId = discipline.Id
        };

        var targetDate = new DateTime(2026, 7, 15, 18, 0, 0, DateTimeKind.Utc);

        var session = new Session
        {
            Id = 2,
            Class = classEntity,
            ClassId = classEntity.Id,
            Instructor = instructor,
            InstructorId = instructor.Id,
            Place = place,
            PlaceId = place.Id,
            Start = targetDate,
            End = targetDate.AddHours(1),
            Slots = 12,
            Enrollments = new List<Enrollment>()
        };

        db.Instructors.Add(instructor);
        db.Disciplines.Add(discipline);
        db.Places.Add(place);
        db.Classes.Add(classEntity);
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        repoMock
            .Setup(x => x.GetAvailableClassesAsync("Pilates", targetDate, false))
            .ReturnsAsync(new List<Session> { session });

        var service = new SessionService(repoMock.Object, db);

        // Act
        var result = await service.GetFilteredSessionsAsync("Pilates", targetDate, false);

        // Assert
        result.Should().HaveCount(1);
        result[0].ClassName.Should().Be("Core Strength");
        result[0].Discipline.Should().Be("Pilates");
    }

    [Fact]
    public async Task GetFilteredSessionsAsync_WhenNoMatchingSessionsExist_ReturnsEmptyList()
    {
        // Arrange
        var repoMock = new Mock<ISessionRepository>();
        await using var db = CreateDbContext();

        repoMock
            .Setup(x => x.GetAvailableClassesAsync("Zumba", It.IsAny<DateTime?>(), false))
            .ReturnsAsync(new List<Session>());

        var service = new SessionService(repoMock.Object, db);

        // Act
        var result = await service.GetFilteredSessionsAsync("Zumba", new DateTime(2026, 7, 15), false);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetFilteredSessionsAsync_WhenDisciplineDoesNotExist_ReturnsEmptyList()
    {
        // Arrange
        var repoMock = new Mock<ISessionRepository>();
        await using var db = CreateDbContext();

        repoMock
            .Setup(x => x.GetAvailableClassesAsync("NotExistingDiscipline", null, false))
            .ReturnsAsync(new List<Session>());

        var service = new SessionService(repoMock.Object, db);

        // Act
        var result = await service.GetFilteredSessionsAsync("NotExistingDiscipline", null, false);

        // Assert
        result.Should().BeEmpty();
    }
}