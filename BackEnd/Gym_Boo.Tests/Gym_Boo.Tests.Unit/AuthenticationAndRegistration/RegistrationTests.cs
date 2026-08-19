using FluentAssertions;
using Gym_Boo.Controllers.Services;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Gym_Boo.Tests.Unit.Registration;

public class UserServiceRegistrationTests
{
    // RegisterMemberAsync_WhenValidInput_ShouldCreateMember
    [Fact]
    public async Task RegisterMemberAsync_WhenValidInput_ShouldCreateMember()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher<User>>();

        Member? capturedMember = null;

        userRepositoryMock
            .Setup(repo => repo.EmailExistsAsync("paolafelix@gymboo.com"))
            .ReturnsAsync(false);

        userRepositoryMock
            .Setup(repo => repo.AddMemberAsync(It.IsAny<Member>()))
            .Callback<Member>(member => capturedMember = member)
            .Returns(Task.CompletedTask);

        passwordHasherMock
            .Setup(hasher => hasher.HashPassword(
                It.IsAny<User>(),
                "Password123!"))
            .Returns("hashed-password");

        var service = new UserService(
            userRepositoryMock.Object,
            passwordHasherMock.Object);

        // Act
        var result = await service.RegisterMemberAsync(
            name: "Paola",
            lastName: "Felix",
            email: "paolafelix@gymboo.com",
            password: "Password123!");

        // Assert
        result.Should().BeNull();

        capturedMember.Should().NotBeNull();
        capturedMember!.Name.Should().Be("Paola");
        capturedMember.LastName.Should().Be("Felix");
        capturedMember.Email.Should().Be("paolafelix@gymboo.com");
        capturedMember.Role.Should().Be(Role.Member);
        capturedMember.IsActive.Should().BeTrue();
        capturedMember.PasswordHash.Should().Be("hashed-password");

        userRepositoryMock.Verify(
            repo => repo.EmailExistsAsync("paolafelix@gymboo.com"),
            Times.Once);

        userRepositoryMock.Verify(
            repo => repo.AddMemberAsync(It.IsAny<Member>()),
            Times.Once);

        passwordHasherMock.Verify(
            hasher => hasher.HashPassword(
                It.IsAny<User>(),
                "Password123!"),
            Times.Once);
    }
    // RegisterMemberAsync_WhenEmailHasWhitespaceAndUppercase_ShouldNormalizeEmail
    [Fact]
    public async Task RegisterMemberAsync_WhenEmailHasWhitespaceAndUppercase_ShouldNormalizeEmail()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher<User>>();

        Member? capturedMember = null;

        userRepositoryMock
            .Setup(repo => repo.EmailExistsAsync("paolafelix@gymboo.com"))
            .ReturnsAsync(false);

        userRepositoryMock
            .Setup(repo => repo.AddMemberAsync(It.IsAny<Member>()))
            .Callback<Member>(member => capturedMember = member)
            .Returns(Task.CompletedTask);

        passwordHasherMock
            .Setup(hasher => hasher.HashPassword(
                It.IsAny<User>(),
                "Password123!"))
            .Returns("hashed-password");

        var service = new UserService(
            userRepositoryMock.Object,
            passwordHasherMock.Object);

        // Act
        var result = await service.RegisterMemberAsync(
            name: "Paola",
            lastName: "Felix",
            email: " PAOLAFELIX@GYMBOO.COM ",
            password: "Password123!");

        // Assert
        result.Should().BeNull();

        capturedMember.Should().NotBeNull();
        capturedMember!.Email.Should().Be("paolafelix@gymboo.com");

        userRepositoryMock.Verify(
            repo => repo.EmailExistsAsync("paolafelix@gymboo.com"),
            Times.Once);

        userRepositoryMock.Verify(
            repo => repo.AddMemberAsync(It.Is<Member>(
                member => member.Email == "paolafelix@gymboo.com")),
            Times.Once);
    }

    // RegisterMemberAsync_WhenNormalizedEmailAlreadyExists_ShouldReturnDuplicateMessageAndNotCreateMember
    [Fact]
    public async Task RegisterMemberAsync_WhenNormalizedEmailAlreadyExists_ShouldReturnDuplicateMessageAndNotCreateMember()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher<User>>();

        userRepositoryMock
            .Setup(repo => repo.EmailExistsAsync("paolafelix@gymboo.com"))
            .ReturnsAsync(true);

        var service = new UserService(
            userRepositoryMock.Object,
            passwordHasherMock.Object);

        // Act
        var result = await service.RegisterMemberAsync(
            name: "Paola",
            lastName: "Felix",
            email: " PAOLAFELIX@GYMBOO.COM ",
            password: "Password123!");

        // Assert
        result.Should().Be("An account with this email already exists.");

        userRepositoryMock.Verify(
            repo => repo.EmailExistsAsync("paolafelix@gymboo.com"),
            Times.Once);

        userRepositoryMock.Verify(
            repo => repo.AddMemberAsync(It.IsAny<Member>()),
            Times.Never);

        passwordHasherMock.Verify(
            hasher => hasher.HashPassword(It.IsAny<User>(), It.IsAny<string>()),
            Times.Never);
    }

    // RegisterMemberAsync_WhenValidInput_ShouldHashPassword
    [Fact]
    public async Task RegisterMemberAsync_WhenValidInput_ShouldHashPassword()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher<User>>();

        userRepositoryMock
            .Setup(repo => repo.EmailExistsAsync("paolafelix@gymboo.com"))
            .ReturnsAsync(false);

        userRepositoryMock
            .Setup(repo => repo.AddMemberAsync(It.IsAny<Member>()))
            .Returns(Task.CompletedTask);

        passwordHasherMock
            .Setup(hasher => hasher.HashPassword(
                It.IsAny<User>(),
                "Password123!"))
            .Returns("hashed-password");

        var service = new UserService(
            userRepositoryMock.Object,
            passwordHasherMock.Object);

        // Act
        var result = await service.RegisterMemberAsync(
            name: "Paola",
            lastName: "Felix",
            email: "paolafelix@gymboo.com",
            password: "Password123!");

        // Assert
        result.Should().BeNull();

        passwordHasherMock.Verify(
            hasher => hasher.HashPassword(
                It.IsAny<User>(),
                "Password123!"),
            Times.Once);

        userRepositoryMock.Verify(
            repo => repo.AddMemberAsync(It.Is<Member>(
                member => member.PasswordHash == "hashed-password")),
            Times.Once);
    }

    // RegisterMemberAsync_WhenNameFieldsHaveWhitespace_ShouldTrimNameAndLastName
    [Fact]
    public async Task RegisterMemberAsync_WhenNameFieldsHaveWhitespace_ShouldTrimNameAndLastName()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher<User>>();

        Member? capturedMember = null;

        userRepositoryMock
            .Setup(repo => repo.EmailExistsAsync("paolafelix@gymboo.com"))
            .ReturnsAsync(false);

        userRepositoryMock
            .Setup(repo => repo.AddMemberAsync(It.IsAny<Member>()))
            .Callback<Member>(member => capturedMember = member)
            .Returns(Task.CompletedTask);

        passwordHasherMock
            .Setup(hasher => hasher.HashPassword(
                It.IsAny<User>(),
                "Password123!"))
            .Returns("hashed-password");

        var service = new UserService(
            userRepositoryMock.Object,
            passwordHasherMock.Object);

        // Act
        var result = await service.RegisterMemberAsync(
            name: "  Paola  ",
            lastName: "  Felix  ",
            email: "paolafelix@gymboo.com",
            password: "Password123!");

        // Assert
        result.Should().BeNull();

        capturedMember.Should().NotBeNull();
        capturedMember!.Name.Should().Be("Paola");
        capturedMember.LastName.Should().Be("Felix");

        userRepositoryMock.Verify(
            repo => repo.AddMemberAsync(It.Is<Member>(
                member => member.Name == "Paola" &&
                         member.LastName == "Felix")),
            Times.Once);
    }

}
