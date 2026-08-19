// REQ-2: Login & Logout
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Gym_Boo.Tests.Integration;

public class LoginIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoginIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WhenValidMemberCredentials_Returns200AndJwt()
    {
        var payload = new
        {
            email = "sarah.brown@gmail.com",
            password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<dynamic>();
        body!.token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WhenValidInstructorCredentials_Returns200AndJwt()
    {
        var payload = new
        {
            email = "james.wilson@gymboo.com",
            password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<dynamic>();
        body!.token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WhenValidAdminCredentials_Returns200AndJwt()
    {
        var payload = new
        {
            email = "admin@gymboo.com",
            password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<dynamic>();
        body!.token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WhenInvalidCredentials_Returns401Unauthorized()
    {
        var payload = new
        {
            email = "admin@gymboo.com",
            password = "wrong-password"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WhenUserIsInactive_Returns403Forbidden()
    {
        var payload = new
        {
            email = "inactive.member@test.com",
            password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Login_WhenInstructorIsInactive_Returns403Forbidden()
    {
        var payload = new
        {
            email = "inactive.instructor@test.com",
            password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Login_WhenUserIsAuthenticated_ReturnsTokenAndUserPayload()
    {
        var payload = new
        {
            email = "sarah.brown@gmail.com",
            password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<dynamic>();
        body!.token.Should().NotBeNullOrWhiteSpace();
        body.user.Should().NotBeNull();
    }
}