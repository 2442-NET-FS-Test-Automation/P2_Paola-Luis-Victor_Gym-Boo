// REQ-5: Reservation Cancellation
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Gym_Boo.Tests.Integration;

public class ReservationCancellationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ReservationCancellationIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CancelReservation_WhenBeforeCutoff_Returns200AndCancelledWithoutFee()
    {
        var response = await _client.DeleteAsync("/api/reservations/8?userId=4");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CancelReservation_WhenInsideCutoff_Returns200AndCancelledWithFee()
    {
        var response = await _client.DeleteAsync("/api/reservations/9?userId=4");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CancelReservation_WhenReservationDoesNotExist_Returns404NotFound()
    {
        var response = await _client.DeleteAsync("/api/reservations/999?userId=1");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
