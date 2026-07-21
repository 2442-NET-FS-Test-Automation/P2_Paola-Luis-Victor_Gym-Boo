using Gym_Boo.ControllerApi.Dtos;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories;

namespace Gym_Boo.ControllerApi.Services;

class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepo;
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly GymBooDbContext _context;

    public ReviewService(IReviewRepository reviewRepo, IEnrollmentRepository enrollmentRepo, GymBooDbContext context)
    {
        _reviewRepo = reviewRepo;
        _enrollmentRepo = enrollmentRepo;
        _context = context;
    }

    public async Task<Review> CreateReviewAsync(string reviewTypeRaw, CreateReviewDto dto)
    {
        // 1. Validate if the provided review type belongs to the Enum
        if (!Enum.TryParse<ReviewType>(reviewTypeRaw, ignoreCase: true, out var reviewType)
            || !Enum.IsDefined(typeof(ReviewType), reviewType))
        {
            throw new ArgumentException($"The review type '{reviewTypeRaw}' is not valid.");
        }

        // 2. Acceptance Criteria: Validate attendance (Status == Attended)
        bool hasAttended = await _enrollmentRepo.EnrollmentHasBeenAttendedAsync(dto.EnrollmentId);
        if (!hasAttended)
        {
            throw new InvalidOperationException("You can only submit a review for classes you have attended.");
        }

        // 3. Create the Review entity
        var review = new Review
        {
            EnrollmentId = dto.EnrollmentId,
            SessionId = dto.SessionId,
            ReviewType = reviewType,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepo.AddAsync(review);
        await _context.SaveChangesAsync();

        return review;
    }
}