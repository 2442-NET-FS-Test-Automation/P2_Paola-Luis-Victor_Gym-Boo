// REQ-2: Login & Logout
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Gym_Boo.Tests.Integration;

public class LoginIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public LoginIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
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

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
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

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
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

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
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
        await EnsureInactiveUserAsync<Member>(
            "inactive.member@test.com",
            Role.Member);

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
        await EnsureInactiveUserAsync<Instructor>(
            "inactive.instructor@test.com",
            Role.Instructor);

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
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
        body.GetProperty("user").ValueKind.Should().Be(JsonValueKind.Object);
    }

    private async Task EnsureInactiveUserAsync<TUser>(string email, Role role)
        where TUser : User, new()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymBooDbContext>();
        if (await db.Users.AnyAsync(u => u.Email == email))
        {
            return;
        }

        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
        var user = new TUser
        {
            Name = "Inactive",
            LastName = role.ToString(),
            Email = email,
            Role = role,
            IsActive = false
        };
        user.PasswordHash = hasher.HashPassword(user, "Password123!");

        db.Users.Add(user);
        await db.SaveChangesAsync();
    }
}
