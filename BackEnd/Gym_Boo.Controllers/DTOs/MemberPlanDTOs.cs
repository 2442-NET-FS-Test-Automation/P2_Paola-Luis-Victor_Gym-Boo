namespace Gym_Boo.ControllerApi.DTOs;

public record MemberPlanChoiceDto(
    int MemberId,
    int PlanId
);

public record MemberPlanUpdateDto(
    int MemberId,
    int CurrentPlanId,
    int NewPlanId
);