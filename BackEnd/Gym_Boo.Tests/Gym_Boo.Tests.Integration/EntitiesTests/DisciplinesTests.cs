using FluentAssertions;
using Gym_Boo.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gym_Boo.Tests.Integration.AdminTests;

public class DisciplinesTests : IDisposable
{
    private const string LiveConnection = 
        "Server=localhost,1433;Database=tempdb;User Id=sa;Password=Vigolpedeneon1;TrustServerCertificate=true";

    private readonly GymBooDbContext _db;
    private IDbContextTransaction _transaction;

    public DisciplinesTests()
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
    
    //Validates that the insertion is done correctly
    [Fact]
    public async Task Create_ShouldPersistDisciplineInDatabase()
    {
        // Arrange
        var discipline = new Discipline { Name = "Test1", Available = true };

        // Act
        await _db.Disciplines.AddAsync(discipline);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear(); // Clear cache to force true DB read

        // Assert
        var saved = await _db.Disciplines.FirstOrDefaultAsync(d => d.Name == "Test1");
        saved.Should().NotBeNull();
        saved!.Available.Should().BeTrue();
    }

    //Validates updates on fields
    [Fact]
    public async Task Update_ShouldModifyExistingDiscipline()
    {
        // Arrange
        var discipline = new Discipline { Name = "Test1", Available = true };
        await _db.Disciplines.AddAsync(discipline);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();

        // Act
        var toUpdate = await _db.Disciplines.FirstAsync(d => d.Name == "Test1");
        toUpdate.Available = false;
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();

        // Assert
        var updated = await _db.Disciplines.FirstAsync(d => d.Name == "Test1");
        updated.Available.Should().BeFalse();
    }

    //Validate that given the name of the Discipline this is erased
    [Fact]
    public async Task Delete_ShouldRemoveDisciplineFromDatabase()
    {
        // Arrange
        var discipline = new Discipline { Name = "Test1", Available = true };
        await _db.Disciplines.AddAsync(discipline);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();

        // Act
        var toDelete = await _db.Disciplines.FirstAsync(d => d.Name == "Test1");
        _db.Disciplines.Remove(toDelete);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();

        // Assert
        var deleted = await _db.Disciplines.FirstOrDefaultAsync(d => d.Name == "Test1");
        deleted.Should().BeNull();
    }

    //Validates that the field "Name" is required
    [Fact]
    public async Task Create_MissingRequiredName_ShouldThrowDbUpdateException()
    {
        // Arrange: Assuming Name is a required column in your DbContext
        var invalidDiscipline = new Discipline { Name = null!, Available = true };

        // Act
        await _db.Disciplines.AddAsync(invalidDiscipline);

        // Assert
        var act = async () => await _db.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }
}