using FluentAssertions;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Tests.Integration.AdminTests;

public class UserTests : IDisposable
{
    private readonly GymBooDbContext _db;
    
    public UserTests()
    {
        var options = new DbContextOptionsBuilder<GymBooDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new GymBooDbContext(options);
    }
    
    public void Dispose()
    {
        _db.Dispose();
    }

    //Instructor related stuff
    //try to create instructor
    [Fact]
    public void TryToCreate_InstructorUser_ShouldReturnTrue()
    {
        //Arrange
        User InsTest = new()
        {
            Email = "email@test.com",
            IsActive = true,
            Name = "Istructor",
            LastName = "Test",
            Role = Role.Instructor,
            PasswordHash = "TestPassword"
        };
        //Act
        _db.Users.Add(InsTest);
        _db.SaveChanges();
        
        //Assert
        _db.Users.Find(InsTest.Id).Should().NotBeNull();
    }

    [Fact]
    public void TryToChange_InstructorState_ShouldReturnTrue()
    {
        // Arrange
        var insTest = new User
        {
            Email = "email@test.com",
            IsActive = false,
            Name = "Instructor",
            LastName = "Test",
            Role = Role.Instructor,
            PasswordHash = "TestPassword"
        };

        _db.Users.Add(insTest);
        _db.SaveChanges();

        // Act
        insTest.IsActive = true;
        _db.Users.Update(insTest);
        _db.SaveChanges(); // Must call SaveChanges to persist the update

        // Assert
        var updatedUser = _db.Users.Find(insTest.Id);
    
        updatedUser.Should().NotBeNull();
        updatedUser!.IsActive.Should().BeTrue();
    }
    
}
