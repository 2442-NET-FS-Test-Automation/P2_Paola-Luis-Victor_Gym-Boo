using GymBoo.Data.Enums;

namespace GymBoo.Data.Entities;

public class SubscriptionPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; } = default;

    public Recurrence Recurrence { get; set; }

    public ICollection<MemberSubscription> MemberSubscriptions { get; set; } = new List<MemberSubscription>();
}