using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;

namespace Gym_Boo.Controllers.DTOs;

public record SeedDataRoot
{
    public List<UserSeedDto> Users { get; set; } = [];
    public List<SubscriptionPlan> SubscriptionPlans { get; set; } = [];
    public List<Place> Places { get; set; } = [];
    public List<Discipline> Disciplines { get; set; } = [];
    public List<ClassSeedDto> Classes { get; set; } = [];
    public List<AvailabilitySeedDto> Availabilities { get; set; } = [];
    public List<SessionSeedDto> Sessions { get; set; } = [];
    public List<MemberSubscriptionSeedDto> MemberSubscriptions { get; set; } = [];
    public List<EnrollmentSeedDto> Enrollments { get; set; } = [];
    public List<ReviewSeedDto> Reviews { get; set; } = [];
}

public record UserSeedDto
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Role Role { get; set; }
    public bool IsActive { get; set; }
    public string SeedPasswordKey { get; set; } = string.Empty;
}

public record ClassSeedDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DisciplineId { get; set; }
}

public record AvailabilitySeedDto
{
    public int InstructorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public record SessionSeedDto
{
    public int ClassId { get; set; }
    public int InstructorId { get; set; }
    public int PlaceId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Slots { get; set; }
    public decimal CancellationFee { get; set; }
}

public record MemberSubscriptionSeedDto
{
    public int MemberId { get; set; }
    public int PlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}

public record EnrollmentSeedDto
{
    public int MemberId { get; set; }
    public int SessionId { get; set; }
    public DateTime EnrollmentDateTime { get; set; }
    public EnrollmentStatus Status { get; set; }
    public bool CancellationFeeApplied { get; set; }
}

public record ReviewSeedDto
{
    public int EnrollmentId { get; set; }
    public int SessionId { get; set; }
    public ReviewType ReviewType { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}