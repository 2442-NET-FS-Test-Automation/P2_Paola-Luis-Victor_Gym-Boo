using FluentAssertions;
using Gym_Boo.Data.Entities;
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
        
        //Act
        
        //Assert
    }
}