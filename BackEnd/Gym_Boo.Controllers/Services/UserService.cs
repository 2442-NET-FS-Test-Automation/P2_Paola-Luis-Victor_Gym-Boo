using Gym_Boo.Data;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.Controllers.Services;

public class UserService : IUserService
{
    private readonly GymBooDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(
        GymBooDbContext db,
        IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<string?> RegisterMemberAsync(
        string name,
        string lastName,
        string email,
        string password)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        bool emailExists = await _db.Users
            .AnyAsync(user => user.Email == normalizedEmail);

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

        member.PasswordHash =
            _passwordHasher.HashPassword(member, password);

        _db.Members.Add(member);

        await _db.SaveChangesAsync();

        return null;
    }

    public async Task<User?> ValidateCredentialsAsync(
        string email,
        string password)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        User? user = await _db.Users
            .SingleOrDefaultAsync(user =>
                user.Email == normalizedEmail);

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