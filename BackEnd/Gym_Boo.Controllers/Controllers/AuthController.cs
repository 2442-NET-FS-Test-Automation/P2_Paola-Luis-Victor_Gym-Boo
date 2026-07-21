using System.ComponentModel.DataAnnotations;

namespace Gym_Boo.Controllers.DTOs;

public record RegisterDto(
    [Required, MaxLength(64)] string Name,
    [Required, MaxLength(64)] string LastName,
    [Required, EmailAddress, MaxLength(150)] string Email,
    [Required, MinLength(8)] string Password
);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);