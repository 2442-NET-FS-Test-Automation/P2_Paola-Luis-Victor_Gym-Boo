using FluentAssertions;
using Gym_Boo.Controllers.Services;
using Gym_Boo.Data;
using Gym_Boo.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client;

namespace Gym_Boo.Tests.Integration.AdminTests;

public class DisciplinesTests : IDisposable
{
    private const string LiveConnection = 
        "Server=localhost,1433;Database=sqlweek;User Id=sa;Password=Gatito_1433!;TrustServerCertificate=true";

    private readonly GymBooDbContext _db;
    private IDbContextTransaction _transaction;

    public DisciplinesTests()
    {

        var options = new DbContextOptionsBuilder<GymBooDbContext>()
            .UseSqlServer(LiveConnection)
            .Options;

        _db = new GymBooDbContext(options);

        // As xUnit creates the object that will run a test method - we will have it
        // start an EF Core transaction
        _transaction = _db.Database.BeginTransaction();
    
    }
    
    public void Dispose()
    {
        _transaction.Rollback(); // every write/edit done by the test is gone
        _transaction.Dispose(); 
        _db.Dispose();
    }

    [Fact]
    public async Task Getting_AllDisciplines_AtLeastOne()
    {
        CancellationToken ct = new CancellationToken();
        //Arrange
        //We dont need to seed data in this test, DB already have a lot
        
        //Act
        var response = await _db.Disciplines.ToListAsync(ct);
        
        //Assert
        response.Count.Should().BeGreaterThanOrEqualTo(1);
    }
    
}