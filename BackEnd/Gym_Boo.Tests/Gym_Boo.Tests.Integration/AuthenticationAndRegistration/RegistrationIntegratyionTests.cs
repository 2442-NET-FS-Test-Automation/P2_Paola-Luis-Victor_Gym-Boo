// REQ-1: Registration
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Gym_Boo.Tests.Integration;

public class RegistrationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegistrationIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WhenValidPayload_Returns201Created()
    {
        var payload = new
        {
            name = "Paola",
            lastName = "Felix",
            email = "paola.integration@test.com",
            password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_WhenDuplicateEmail_Returns400BadRequest()
    {
        var payload = new
        {
            name = "Paola",
            lastName = "Felix",
            email = "duplicate@test.com",
            password = "Password123!"
        };

        await _client.PostAsJsonAsync("/api/auth/register", payload);
        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WhenEmailIsNormalized_ReturnsBadRequestForDuplicateNormalizedEmail()
    {
        var payload1 = new
        {
            name = "Paola",
            lastName = "Felix",
            email = "paola@test.com",
            password = "Password123!"
        };

        var payload2 = new
        {
            name = "Paola",
            lastName = "Felix",
            email = "PAOLA@TEST.COM",
            password = "Password123!"
        };

        await _client.PostAsJsonAsync("/api/auth/register", payload1);
        var response = await _client.PostAsJsonAsync("/api/auth/register", payload2);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WhenInvalidData_Returns400BadRequest()
    {
        var payload = new
        {
            name = "",
            lastName = "",
            email = "",
            password = ""
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}