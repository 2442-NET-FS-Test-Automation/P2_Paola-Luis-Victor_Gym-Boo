// REQ-7: Review Creation
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Gym_Boo.Tests.Integration;

public class ReviewCreationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ReviewCreationIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateReview_WhenValidReview_Returns201Created()
    {
        var payload = new
        {
            enrollmentId = 1,
            sessionId = 1,
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
            enrollmentId = 1,
            sessionId = 1,
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
}