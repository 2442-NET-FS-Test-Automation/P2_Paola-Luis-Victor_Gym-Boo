using Gym_Boo.Controllers.DTOs;
using Gym_Boo.Controllers.Services;
using Gym_Boo.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gym_Boo.Controllers.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _users;
    private readonly ITokenService _tokens;

    public AuthController(
        IUserService users,
        ITokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        string? error = await _users.RegisterMemberAsync(
            dto.Name,
            dto.LastName,
            dto.Email,
            dto.Password);

        if (error is not null)
        {
            return BadRequest(new { message = error });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            new
            {
                message = "Member account created successfully."
            });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _users.ValidateCredentialsAsync(
            dto.Email,
            dto.Password);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        if (user.Role == Role.Instructor && !user.IsActive)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    message =
                        "Your instructor account is inactive. Please contact an administrator."
                });
        }

        if (!user.IsActive)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    message = "Your account is inactive."
                });
        }

        string token = _tokens.Issue(user);

        return Ok(new
        {
            message = "Login successful.",
            token,
            user = new
            {
                user.Id,
                user.Name,
                user.LastName,
                user.Email,
                role = user.Role.ToString()
            }
        });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(new
        {
            id = User.FindFirstValue(
                ClaimTypes.NameIdentifier),
            email = User.Identity?.Name,
            role = User.FindFirstValue(
                ClaimTypes.Role)
        });
    }
}