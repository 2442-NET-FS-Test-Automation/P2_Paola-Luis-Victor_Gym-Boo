using FluentAssertions;
using Gym_Boo.Controllers.Services;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Gym_Boo.Tests.Unit.AuthenticationAndRegistration;

public class UserServiceLoginTests
{
    // ValidateCredentialsAsync_WhenUserExistsAndPasswordMatches_ReturnsUser
    [Fact]
    public async Task ValidateCredentialsAsync_WhenUserExistsAndPasswordMatches_ReturnsUser()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher<User>>();

        var user = new User
        {
            Id = 1,
            Name = "Paola",
            LastName = "Felix",
            Email = "paola@gymboo.com",
            Role = Role.Member,
            PasswordHash = "hashed-password"
        };

        userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync("paola@gymboo.com"))
            .ReturnsAsync(user);

        passwordHasherMock
            .Setup(hasher => hasher.VerifyHashedPassword(
                user,
                "hashed-password",
                "Password123!"))
            .Returns(PasswordVerificationResult.Success);

        var service = new UserService(
            userRepositoryMock.Object,
            passwordHasherMock.Object);

        // Act
        var result = await service.ValidateCredentialsAsync(
            "paola@gymboo.com",
            "Password123!");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("paola@gymboo.com");
        result.Role.Should().Be(Role.Member);
    }

    // ValidateCredentialsAsync_WhenUserDoesNotExist_ReturnsNull
    [Fact]
    public async Task ValidateCredentialsAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher<User>>();

        userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync("missing@gymboo.com"))
            .ReturnsAsync((User?)null);

        var service = new UserService(
            userRepositoryMock.Object,
            passwordHasherMock.Object);

        // Act
        var result = await service.ValidateCredentialsAsync(
            "missing@gymboo.com",
            "Password123!");

        // Assert
        result.Should().BeNull();
    }

    // ValidateCredentialsAsync_WhenPasswordIsIncorrect_ReturnsNull
    [Fact]
    public async Task ValidateCredentialsAsync_WhenPasswordIsIncorrect_ReturnsNull()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher<User>>();

        var user = new User
        {
            Id = 2,
            Name = "Luis",
            LastName = "Valencia",
            Email = "luis@gymboo.com",
            Role = Role.Instructor,
            PasswordHash = "hashed-password"
        };

        userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync("luis@gymboo.com"))
            .ReturnsAsync(user);

        passwordHasherMock
            .Setup(hasher => hasher.VerifyHashedPassword(
                user,
                "hashed-password",
                "WrongPassword"))
            .Returns(PasswordVerificationResult.Failed);

        var service = new UserService(
            userRepositoryMock.Object,
            passwordHasherMock.Object);

        // Act
        var result = await service.ValidateCredentialsAsync(
            "luis@gymboo.com",
            "WrongPassword");

        // Assert
        result.Should().BeNull();
    }

    // Issue_WhenUserIsProvided_ReturnsJwtToken
    [Fact]
    public void Issue_WhenUserIsProvided_ReturnsJwtToken()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "super-secret-key-for-gymboo-123456!",
                ["Jwt:Issuer"] = "GymBoo",
                ["Jwt:Audience"] = "GymBooClient"
            })
            .Build();

        var service = new TokenService(config);

        var user = new User
        {
            Id = 5,
            Name = "Iyari",
            LastName = "Flores",
            Email = "victor@gymboo.com",
            Role = Role.Admin
        };

        // Act
        var token = service.Issue(user);

        // Assert
        token.Should().NotBeNullOrWhiteSpace();
        token.Should().Contain(".");
    }

    // Issue_WhenUserIsProvided_IncludesRoleClaim
    [Fact]
    public void Issue_WhenUserIsProvided_IncludesRoleClaim()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "super-secret-key-for-gymboo-123456!",
                ["Jwt:Issuer"] = "GymBoo",
                ["Jwt:Audience"] = "GymBooClient"
            })
            .Build();

        var service = new TokenService(config);

        var user = new User
        {
            Id = 7,
            Name = "Admin",
            LastName = "User",
            Email = "admin@gymboo.com",
            Role = Role.Admin
        };

        // Act
        var token = service.Issue(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Should().Contain(c =>
            c.Type == ClaimTypes.Role &&
            c.Value == Role.Admin.ToString());
    }
}