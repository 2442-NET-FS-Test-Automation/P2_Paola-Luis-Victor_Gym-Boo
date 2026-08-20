using Gym_Boo.Controllers.Services;
using Gym_Boo.Data.DTOs;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Gym_Boo.Tests.Integration.AdminTests;

public class AdminServicesTests
{
    private readonly Mock<IAdminRepository> _repoMock;
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
    private readonly AdminServices _service;

    public AdminServicesTests()
    {
        _repoMock = new Mock<IAdminRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher<User>>();
        _passwordHasherMock
            .Setup(h => h.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
            .Returns("hashed-password");

        _service = new AdminServices(_repoMock.Object, _passwordHasherMock.Object);
    }

    #region Disciplines Tests

    [Fact]
    public async Task GetAllDisciplines_ReturnsDisciplinesList()
    {
        // Arrange
        var expected = new List<Discipline> { new Discipline { Id = 1, Name = "yoga" } };
        _repoMock.Setup(r => r.GetAllDisciplinesAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(expected);

        // Act
        var result = await _service.GetAllDisciplines(CancellationToken.None);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task NewDisciplineAsync_NullOrWhitespaceName_ThrowsArgumentException(string invalidName)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.NewDisciplineAsync(invalidName, CancellationToken.None));
    }

    [Fact]
    public async Task NewDisciplineAsync_ExistingName_ThrowsInvalidOperationException()
    {
        // Arrange
        _repoMock.Setup(r => r.DisciplineExistsByNameAsync("boxing", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.NewDisciplineAsync("  Boxing  ", CancellationToken.None));
    }

    [Fact]
    public async Task NewDisciplineAsync_ValidName_AddsAndReturnsDiscipline()
    {
        // Arrange
        _repoMock.Setup(r => r.DisciplineExistsByNameAsync("boxing", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

        // Act
        var result = await _service.NewDisciplineAsync("  Boxing  ", CancellationToken.None);

        // Assert
        Assert.Equal("boxing", result.Name);
        Assert.True(result.Available);
        _repoMock.Verify(r => r.AddDisciplineAsync(It.Is<Discipline>(d => d.Name == "boxing"), It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateDiscipline_NullOrWhitespaceName_ThrowsArgumentException(string invalidName)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.UpdateDiscipline(1, invalidName, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateDiscipline_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _repoMock.Setup(r => r.UpdateDisciplineNameAsync(1, "pilates", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.UpdateDiscipline(1, " Pilates ", CancellationToken.None));
    }

    [Fact]
    public async Task UpdateDiscipline_Valid_ExecutesSuccessfully()
    {
        // Arrange
        _repoMock.Setup(r => r.UpdateDisciplineNameAsync(1, "pilates", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

        // Act
        await _service.UpdateDiscipline(1, " Pilates ", CancellationToken.None);

        // Assert
        _repoMock.Verify(r => r.UpdateDisciplineNameAsync(1, "pilates", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleDiscipline_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _repoMock.Setup(r => r.ToggleDisciplineAvailabilityAsync(1, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.ToggleDiscipline(1, CancellationToken.None));
    }

    [Fact]
    public async Task ToggleDiscipline_Valid_ExecutesSuccessfully()
    {
        // Arrange
        _repoMock.Setup(r => r.ToggleDisciplineAvailabilityAsync(1, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

        // Act
        await _service.ToggleDiscipline(1, CancellationToken.None);

        // Assert
        _repoMock.Verify(r => r.ToggleDisciplineAvailabilityAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task DeleteDiscipline_NullOrWhitespace_ThrowsArgumentException(string invalidName)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.DeleteDiscipline(invalidName, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteDiscipline_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _repoMock.Setup(r => r.DeleteDisciplineByNameAsync("zumba", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.DeleteDiscipline(" Zumba ", CancellationToken.None));
    }

    [Fact]
    public async Task DeleteDiscipline_Valid_ExecutesSuccessfully()
    {
        // Arrange
        _repoMock.Setup(r => r.DeleteDisciplineByNameAsync("zumba", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

        // Act
        await _service.DeleteDiscipline(" Zumba ", CancellationToken.None);

        // Assert
        _repoMock.Verify(r => r.DeleteDisciplineByNameAsync("zumba", It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Instructors Tests

    [Fact]
    public async Task GetAllInstructors_ReturnsInstructorsList()
    {
        // Arrange
        var expected = new List<User> { new User { Id = 1, Role = Role.Instructor } };
        _repoMock.Setup(r => r.GetUsersByRoleAsync(Role.Instructor, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(expected);

        // Act
        var result = await _service.GetAllInstructors(CancellationToken.None);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetInstructorById_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _repoMock.Setup(r => r.GetUserByIdAndRoleAsync(1, Role.Instructor, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.GetInstructorById(1, CancellationToken.None));
    }

    [Fact]
    public async Task GetInstructorById_Found_ReturnsInstructor()
    {
        // Arrange
        var instructor = new User { Id = 1, Role = Role.Instructor };
        _repoMock.Setup(r => r.GetUserByIdAndRoleAsync(1, Role.Instructor, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(instructor);

        // Act
        var result = await _service.GetInstructorById(1, CancellationToken.None);

        // Assert
        Assert.Equal(instructor, result);
    }

    [Fact]
    public async Task NewInstructor_NullDtoOrEmptyEmail_ThrowsArgumentException()
    {
        // Arrange
        var invalidDto = new CreateInstructorDto("John", "Doe", "", "123456789");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.NewInstructor(null!, CancellationToken.None));

        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.NewInstructor(invalidDto, CancellationToken.None));
    }

    [Fact]
    public async Task NewInstructor_EmailExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new CreateInstructorDto("John", "Doe", "john@test.com", "123456789");
        _repoMock.Setup(r => r.UserExistsByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.NewInstructor(dto, CancellationToken.None));
    }

    [Fact]
    public async Task NewInstructor_ValidDto_CreatesAndReturnsUser()
    {
        // Arrange
        var dto = new CreateInstructorDto("John", "Doe", "john@test.com", "123456789");
        _repoMock.Setup(r => r.UserExistsByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

        // Act
        var result = await _service.NewInstructor(dto, CancellationToken.None);

        // Assert
        Assert.Equal("john@test.com", result.Email);
        Assert.Equal("John", result.Name);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal(Role.Instructor, result.Role);
        Assert.True(result.IsActive);
        Assert.Equal("hashed-password", result.PasswordHash);
        _repoMock.Verify(r => r.AddUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateInstructor_NullDto_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _service.UpdateInstructor(null!, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateInstructor_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dto = new UpdateInstructorDto(1, "Jane", "Doe", "jane@test.com", true);
        _repoMock.Setup(r => r.GetUserByIdAndRoleAsync(dto.Id, Role.Instructor, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.UpdateInstructor(dto, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateInstructor_ValidDto_UpdatesAndSaves()
    {
        // Arrange
        var dto = new UpdateInstructorDto(1, "Jane", "Smith", "jane.smith@test.com", false);
        var existingUser = new User { Id = 1, Name = "John", LastName = "Doe", Email = "john@test.com", Role = Role.Instructor };

        _repoMock.Setup(r => r.GetUserByIdAndRoleAsync(dto.Id, Role.Instructor, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(existingUser);

        // Act
        await _service.UpdateInstructor(dto, CancellationToken.None);

        // Assert
        Assert.Equal("Jane", existingUser.Name);
        Assert.Equal("Smith", existingUser.LastName);
        Assert.Equal("jane.smith@test.com", existingUser.Email);
        Assert.False(existingUser.IsActive);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteInstructor_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _repoMock.Setup(r => r.DeleteUserByIdAndRoleAsync(1, Role.Instructor, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.DeleteInstructor(1, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteInstructor_ValidId_ExecutesSuccessfully()
    {
        // Arrange
        _repoMock.Setup(r => r.DeleteUserByIdAndRoleAsync(1, Role.Instructor, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

        // Act
        await _service.DeleteInstructor(1, CancellationToken.None);

        // Assert
        _repoMock.Verify(r => r.DeleteUserByIdAndRoleAsync(1, Role.Instructor, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Reports & Analytics Tests

    [Fact]
    public async Task MostPopularClass_DelegatesToRepoWithLimitFive()
    {
        // Arrange
        var expected = new List<MostRatedDto>();
        _repoMock.Setup(r => r.GetMostPopularClassesAsync(5, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(expected);

        // Act
        var result = await _service.MostPopularClass(CancellationToken.None);

        // Assert
        Assert.Equal(expected, result);
        _repoMock.Verify(r => r.GetMostPopularClassesAsync(5, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrationReports_DelegatesToRepo()
    {
        // Arrange
        var expected = new List<DisciplineReportDto>();
        _repoMock.Setup(r => r.GetRegistrationReportsAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(expected);

        // Act
        var result = await _service.RegistrationReports(CancellationToken.None);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TotalRevenue_CalculatesAndReturnsRevenueReportDto()
    {
        // Arrange
        decimal cancellationRevenue = 150.00m;
        decimal subscriptionRevenue = 1000.00m;

        _repoMock.Setup(r => r.GetCancellationRevenueAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(cancellationRevenue);
        _repoMock.Setup(r => r.GetSubscriptionRevenueAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(subscriptionRevenue);

        // Act
        var result = await _service.TotalRevenue(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _repoMock.Verify(r => r.GetCancellationRevenueAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.GetSubscriptionRevenueAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
