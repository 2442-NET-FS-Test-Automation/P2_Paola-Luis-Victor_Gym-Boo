using System.ComponentModel.DataAnnotations;

namespace GymBoo.ControllerApi.DTOs;

public record TakingAttendanceDTO(
    int EnrollmentId,

    [AllowedValues("attended", "not attended", ErrorMessage = "Error: Unsupported action.")] string Action
);