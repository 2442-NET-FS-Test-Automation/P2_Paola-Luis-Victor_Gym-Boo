namespace GymBoo.Data.Entities;

public class MemberSubscription
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public int PlanId { get; set; }
    public SubscriptionPlan Plan { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}