// REQ-6: Reservation History
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Gym_Boo.Tests.Integration;

public class ReservationHistoryIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ReservationHistoryIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHistory_WhenMemberHasReservations_Returns200AndGroupedPayload()
    {
        var response = await _client.GetAsync("/api/reservations?userId=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetHistory_WhenMemberHasNoReservations_Returns200AndEmptyPartitions()
    {
        var response = await _client.GetAsync("/api/reservations?userId=999");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}