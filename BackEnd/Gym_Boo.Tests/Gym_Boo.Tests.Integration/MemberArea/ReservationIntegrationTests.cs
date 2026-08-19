// REQ-4: Class Reservation
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Gym_Boo.Tests.Integration;

public class ReservationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ReservationIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ReserveClass_WhenValidInput_Returns201Created()
    {
        var payload = new
        {
            sessionId = 1,
            memberId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/reservations", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task ReserveClass_WhenClassDoesNotExist_Returns400BadRequest()
    {
        var payload = new
        {
            sessionId = 9999,
            memberId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/reservations", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReserveClass_WhenMemberDoesNotExist_Returns400BadRequest()
    {
        var payload = new
        {
            sessionId = 1,
            memberId = 9999
        };

        var response = await _client.PostAsJsonAsync("/api/reservations", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}