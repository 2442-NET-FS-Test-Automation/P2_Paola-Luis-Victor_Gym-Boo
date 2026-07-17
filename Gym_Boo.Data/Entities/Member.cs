namespace GymBoo.Data.Entities;

public class Member : User
{
    public MemberSubscription? MemberSubscription { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}