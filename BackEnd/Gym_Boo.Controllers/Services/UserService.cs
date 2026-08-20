using Gym_Boo.Data;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Gym_Boo.Controllers.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(
        IUserRepository users,
        IPasswordHasher<User> passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }

    public async Task<string?> RegisterMemberAsync(
        string name,
        string lastName,
        string email,
        string password)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        bool emailExists = await _users.EmailExistsAsync(normalizedEmail);

        if (emailExists)
        {
            return "An account with this email already exists.";
        }

        var member = new Member
        {
            Name = name.Trim(),
            LastName = lastName.Trim(),
            Email = normalizedEmail,
            Role = Role.Member,
            IsActive = true
        };

        member.PasswordHash = _passwordHasher.HashPassword(member, password);

        await _users.AddMemberAsync(member);

        return null;
    }

    public async Task<User?> ValidateCredentialsAsync(
        string email,
        string password)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        User? user = await _users.GetByEmailAsync(normalizedEmail);

        if (user is null)
        {
            return null;
        }

        PasswordVerificationResult result =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

        return result == PasswordVerificationResult.Failed
            ? null
            : user;
    }
}