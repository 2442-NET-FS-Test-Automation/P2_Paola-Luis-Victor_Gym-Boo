using Gym_Boo.ControllerApi.DTOs;
using Gym_Boo.Data.Entities;
using Gym_Boo.Data.Enums;
using Gym_Boo.Data.Repositories;

namespace Gym_Boo.ControllerApi.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepo;
    private readonly IPlanRepository _planRepo;

    public MemberService(IMemberRepository memberRepo, IPlanRepository planRepo)
    {
        _memberRepo = memberRepo;
        _planRepo = planRepo;
    }

    public async Task<MemberReportDto?> GetMemberReportAsync(int memberId)
    {
        var member = await _memberRepo.GetByIdWithReportDataAsync(memberId);
        if (member is null) return null;

        // Calculation of metrics
        int classesAttended = member.Enrollments?.Count(e => e.Status == EnrollmentStatus.Attended) ?? 0;

        var memberReviews = member.Enrollments?
            .SelectMany(e => e.Reviews ?? Enumerable.Empty<Review>())
            .ToList();

        decimal avgReviews = memberReviews != null && memberReviews.Any()
            ? Math.Round((decimal)memberReviews.Average(r => r.Rating), 1)
            : 0.0m;

        return new MemberReportDto(
            Id: member.Id,
            Name: member.Name,
            LastName: member.LastName,
            Email: member.Email,
            UserType: member.Role.ToString() ?? "Member",
            IsActive: member.IsActive,
            IdSubscription: member.MemberSubscription?.Id,
            PlanId: member.MemberSubscription?.PlanId,
            StartTime: member.MemberSubscription != null
            ? DateTime.SpecifyKind(member.MemberSubscription.StartDate, DateTimeKind.Utc)
            : null,
            EndTime: member.MemberSubscription != null
            ? DateTime.SpecifyKind(member.MemberSubscription.ExpirationDate, DateTimeKind.Utc)
            : null,
            ClassesAttended: classesAttended,
            AvgReviews: avgReviews
        );
    }
}