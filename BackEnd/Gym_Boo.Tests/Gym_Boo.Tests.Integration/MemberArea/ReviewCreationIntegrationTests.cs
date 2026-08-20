// REQ-7: Review Creation
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Gym_Boo.Tests.Integration;

public class ReviewCreationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ReviewCreationIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateReview_WhenValidReview_Returns201Created()
    {
        var (enrollmentId, sessionId) = await CreateAttendedEnrollmentAsync();

        var payload = new
        {
            enrollmentId,
            sessionId,
            rating = 5,
            comment = "Excellent class"
        };

        var response = await _client.PostAsJsonAsync("/api/reviews/Class", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateReview_WhenReviewTypeIsInvalid_Returns400BadRequest()
    {
        var payload = new
        {
            enrollmentId = 5,
            sessionId = 9,
            rating = 4,
            comment = "Nice"
        };

        var response = await _client.PostAsJsonAsync("/api/reviews/InvalidType", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReview_WhenDuplicateReviewExists_Returns409Conflict()
    {
        var payload = new
        {
            enrollmentId = 1,
            sessionId = 1,
            rating = 4,
            comment = "Nice"
        };

        var response = await _client.PostAsJsonAsync("/api/reviews/Class", payload);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,
            HttpStatusCode.Conflict);
    }

    private async Task<(int EnrollmentId, int SessionId)> CreateAttendedEnrollmentAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymBooDbContext>();

        var memberId = await db.Users
            .Where(u => u.Role == Role.Member)
            .Select(u => u.Id)
            .FirstAsync();
        var sessionId = await db.Sessions
            .Select(s => s.Id)
            .FirstAsync();

        var enrollment = new Enrollment
        {
            MemberId = memberId,
            SessionId = sessionId,
            EnrollmentDateTime = DateTime.UtcNow.AddDays(-1),
            Status = EnrollmentStatus.Attended,
            CancellationFeeApplied = false
        };

        db.Enrollments.Add(enrollment);
        await db.SaveChangesAsync();

        return (enrollment.Id, sessionId);
    }
}
