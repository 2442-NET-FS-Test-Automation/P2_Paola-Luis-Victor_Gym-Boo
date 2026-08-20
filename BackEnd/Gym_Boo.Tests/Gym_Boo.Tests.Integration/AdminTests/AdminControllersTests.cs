using Gym_Boo.Controllers.Controllers;
using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Controllers.Services.Interfaces;
using Gym_Boo.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Gym_Boo.Tests.Integration.AdminTests;

public class AdminControllerTests
{
private readonly Mock<IAdminServices> _adminServicesMock;
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        _adminServicesMock = new Mock<IAdminServices>();
        _controller = new AdminController(_adminServicesMock.Object);
    }

    [Fact]
    public async Task GetDisciplinesList_ReturnsOkObjectResult_WithDisciplines()
    {
        // Arrange
        var expectedDisciplines = new List<Discipline>
        {
            new Discipline { Id = 1, Name = "Yoga" },
            new Discipline { Id = 2, Name = "Pilates" }
        };

        _adminServicesMock
            .Setup(s => s.GetAllDisciplines(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDisciplines);

        // Act
        var result = await _controller.GetDisciplinesList(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(expectedDisciplines, okResult.Value);
    }

    [Fact]
    public async Task CreateDiscipline_ReturnsStatusCode201_WithCreatedDiscipline()
    {
        // Arrange
        var dto = new DisciplineDto("Boxing");
        var createdDiscipline = new Discipline { Id = 1, Name = "Boxing" };

        _adminServicesMock
            .Setup(s => s.NewDisciplineAsync(dto.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdDiscipline);

        // Act
        var result = await _controller.CreateDiscipline(dto, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
        Assert.Equal(createdDiscipline, objectResult.Value);
    }

    [Fact]
    public async Task UpdateDiscipline_ReturnsOkResult_AndCallsService()
    {
        // Arrange
        int disciplineId = 5;
        var dto = new DisciplineDto("Updated Yoga");

        _adminServicesMock
            .Setup(s => s.UpdateDiscipline(disciplineId, dto.Name, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateDiscipline(disciplineId, dto, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        _adminServicesMock.Verify(s => s.UpdateDiscipline(disciplineId, dto.Name, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleDisciplineStatus_ReturnsOkResult_AndCallsService()
    {
        // Arrange
        int disciplineId = 3;

        _adminServicesMock
            .Setup(s => s.ToggleDiscipline(disciplineId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ToggleDisciplineStatus(disciplineId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        _adminServicesMock.Verify(s => s.ToggleDiscipline(disciplineId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteDiscipline_ReturnsOkResult_AndCallsServiceWithName()
    {
        // Arrange
        string disciplineName = "Boxing";

        _adminServicesMock
            .Setup(s => s.DeleteDiscipline(disciplineName, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteDiscipline(disciplineName, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        _adminServicesMock.Verify(s => s.DeleteDiscipline(disciplineName, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetInstructors_ReturnsOkObjectResult_WithInstructors()
    {
        // Arrange
        var mockInstructors = new List<User>
        {
            new User { Id = 1, Name = "John Doe" }
        };

        _adminServicesMock
            .Setup(s => s.GetAllInstructors(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockInstructors);

        // Act
        var result = await _controller.GetInstructors(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(mockInstructors, okResult.Value);
    }

    [Fact]
    public async Task GetInstructor_ReturnsOkObjectResult_WithInstructorDetails()
    {
        // Arrange
        int instructorId = 10;
        var mockInstructor = new User { Id = instructorId, Name = "Jane Doe" };

        _adminServicesMock
            .Setup(s => s.GetInstructorById(instructorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockInstructor);

        // Act
        var result = await _controller.GetInstructor(instructorId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(mockInstructor, okResult.Value);
    }

    [Fact]
    public async Task CreateInstructor_ReturnsCreatedAtActionResult_WhenSuccessful()
    {
        // Arrange
        var createDto = new Gym_Boo.Data.DTOs.CreateInstructorDto("John", "Doe", "john@test.com", "123456789");
        var createdUser = new User { Id = 42, Name = "John" };

        _adminServicesMock
            .Setup(s => s.NewInstructor(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _controller.CreateInstructor(createDto, CancellationToken.None);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(AdminController.GetInstructor), createdAtActionResult.ActionName);
        Assert.Equal(42, createdAtActionResult.RouteValues?["id"]);
        Assert.Equal(createdUser, createdAtActionResult.Value);
    }

    [Fact]
    public async Task UpdateInstructor_ReturnsBadRequest_WhenRouteIdDoesNotMatchDtoId()
    {
        // Arrange
        int routeId = 1;
        var updateDto = new Gym_Boo.Data.DTOs.UpdateInstructorDto(2, "Jane", "Doe", "jane@test.com", true); // Id = 2

        // Act
        var result = await _controller.UpdateInstructor(routeId, updateDto, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        _adminServicesMock.Verify(s => s.UpdateInstructor(It.IsAny<Gym_Boo.Data.DTOs.UpdateInstructorDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateInstructor_ReturnsOkResult_WhenIdsMatch()
    {
        // Arrange
        int instructorId = 1;
        var updateDto = new Gym_Boo.Data.DTOs.UpdateInstructorDto(1, "Jane", "Doe", "jane@test.com", true); // Id = 1

        _adminServicesMock
            .Setup(s => s.UpdateInstructor(updateDto, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateInstructor(instructorId, updateDto, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        _adminServicesMock.Verify(s => s.UpdateInstructor(updateDto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveInstructor_ReturnsOkResult_AndCallsService()
    {
        // Arrange
        int instructorId = 12;

        _adminServicesMock
            .Setup(s => s.DeleteInstructor(instructorId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RemoveInstructor(instructorId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        _adminServicesMock.Verify(s => s.DeleteInstructor(instructorId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetSessionsReport_ReturnsOkResult_WithReportData()
    {
        // Arrange
        var mockReport = new List<Gym_Boo.Data.DTOs.DisciplineReportDto>();

        _adminServicesMock
            .Setup(s => s.RegistrationReports(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockReport);

        // Act
        var result = await _controller.GetSessionsReport(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(mockReport, okResult.Value);
    }

    [Fact]
    public async Task GetRevenueReport_ReturnsOkResult_WithRevenueData()
    {
        // Arrange
        // Passing 3 dummy arguments to satisfy the 3-parameter primary constructor
        var mockRevenue = new Gym_Boo.Data.DTOs.RevenueReportDto(0, 0, 0);

        _adminServicesMock
            .Setup(s => s.TotalRevenue(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockRevenue);

        // Act
        var result = await _controller.GetRevenueReport(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(mockRevenue, okResult.Value);
    }
    
    [Fact]
    public async Task GetMostPopularReport_ReturnsOkResult_WithPopularClassData()
    {
        // Arrange
        var mockPopular = new List<Gym_Boo.Data.DTOs.MostRatedDto>();

        _adminServicesMock
            .Setup(s => s.MostPopularClass(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockPopular);

        // Act
        var result = await _controller.GetMostPopularReport(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(mockPopular, okResult.Value);
    }
}
