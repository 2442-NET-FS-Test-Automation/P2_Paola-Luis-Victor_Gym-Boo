// REQ-3: Class Search & Browsing
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Gym_Boo.Tests.Integration;

public class ClassSearchIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ClassSearchIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetClasses_WhenNoFilters_Returns200AndList()
    {
        var response = await _client.GetAsync("/api/classes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetClasses_WhenValidFilters_Returns200AndMatchingResults()
    {
        var response = await _client.GetAsync("/api/classes?discipline=Yoga&date=2026-07-15");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetClasses_WhenNoResults_Returns200AndEmptyList()
    {
        var response = await _client.GetAsync("/api/classes?discipline=NonExistingDiscipline");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetClasses_WhenInvalidDate_Returns400BadRequest()
    {
        var response = await _client.GetAsync("/api/classes?date=not-a-date");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}