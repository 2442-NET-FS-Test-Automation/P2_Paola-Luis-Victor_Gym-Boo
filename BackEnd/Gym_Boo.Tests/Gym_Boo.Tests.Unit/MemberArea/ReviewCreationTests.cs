// REQ-7: Review Creation
using FluentAssertions;
using Gym_Boo.ControllerApi.DTOs;
using Gym_Boo.ControllerApi.Exceptions;
using Gym_Boo.ControllerApi.Services;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Gym_Boo.Tests.Unit.MemberArea;

public class ReviewCreationServiceTests
{
    private static GymBooDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<GymBooDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GymBooDbContext(options);
    }

    [Fact]
    public async Task CreateReviewAsync_WhenValidReview_ReturnsCreated()
    {
        var reviewRepo = new Mock<IReviewRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        await using var db = CreateDbContext();

        var dto = new CreateReviewDto(7, 20, 5, "Excellent class");

        reviewRepo.Setup(x => x.ExistAsync(dto.EnrollmentId, ReviewType.Class)).ReturnsAsync(false);
        enrollmentRepo.Setup(x => x.EnrollmentHasBeenAttendedAsync(dto.EnrollmentId)).ReturnsAsync(true);

        Review? createdReview = null;
        reviewRepo.Setup(x => x.AddAsync(It.IsAny<Review>()))
            .Callback<Review>(review => createdReview = review)
            .Returns(Task.CompletedTask);

        var service = new ReviewService(reviewRepo.Object, enrollmentRepo.Object, db);

        var result = await service.CreateReviewAsync("Class", dto);

        result.Should().NotBeNull();
        result.ReviewType.Should().Be(ReviewType.Class);
        result.Rating.Should().Be(5);
        result.Comment.Should().Be("Excellent class");
        createdReview.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateReviewAsync_WhenReviewTypeIsInvalid_ThrowsArgumentException()
    {
        var reviewRepo = new Mock<IReviewRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        await using var db = CreateDbContext();

        var dto = new CreateReviewDto(7, 20, 4, "Nice");

        var service = new ReviewService(reviewRepo.Object, enrollmentRepo.Object, db);

        var act = async () => await service.CreateReviewAsync("NotAType", dto);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateReviewAsync_WhenReviewAlreadyExists_ThrowsDuplicateReviewException()
    {
        var reviewRepo = new Mock<IReviewRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        await using var db = CreateDbContext();

        var dto = new CreateReviewDto(7, 20, 4, "Nice");
        reviewRepo.Setup(x => x.ExistAsync(dto.EnrollmentId, ReviewType.Class)).ReturnsAsync(true);

        var service = new ReviewService(reviewRepo.Object, enrollmentRepo.Object, db);

        var act = async () => await service.CreateReviewAsync("Class", dto);

        await act.Should().ThrowAsync<DuplicateReviewException>();
    }

    [Fact]
    public async Task CreateReviewAsync_WhenAttendanceWasNotConfirmed_ThrowsInvalidOperationException()
    {
        var reviewRepo = new Mock<IReviewRepository>();
        var enrollmentRepo = new Mock<IEnrollmentRepository>();
        await using var db = CreateDbContext();

        var dto = new CreateReviewDto(7, 20, 4, "Nice");
        reviewRepo.Setup(x => x.ExistAsync(dto.EnrollmentId, ReviewType.Class)).ReturnsAsync(false);
        enrollmentRepo.Setup(x => x.EnrollmentHasBeenAttendedAsync(dto.EnrollmentId)).ReturnsAsync(false);

        var service = new ReviewService(reviewRepo.Object, enrollmentRepo.Object, db);

        var act = async () => await service.CreateReviewAsync("Class", dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*attended*");
    }
}