using FluentAssertions;
using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gym_Boo.Tests.Integration.AdminTests;

public class UserTests
{
    private const string LiveConnection = 
        "Server=localhost,1433;Database=tempdb;User Id=sa;Password=Vigolpedeneon1;TrustServerCertificate=true";

    private readonly GymBooDbContext _db;
    private IDbContextTransaction _transaction;
    
    public UserTests()
    {

        var options = new DbContextOptionsBuilder<GymBooDbContext>()
            .UseSqlServer(LiveConnection)
            .Options;

        _db = new GymBooDbContext(options);
        
        
        _transaction = _db.Database.BeginTransaction();
    
    }
    
    public void Dispose()
    {
        _transaction.Rollback(); // every write/edit done by the test is gone
        _transaction.Dispose(); 
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
        _db.Users.Find(InsTest).Should().NotBeNull();
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