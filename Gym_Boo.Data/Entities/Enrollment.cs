using GymBoo.Data.Enums;

namespace GymBoo.Data.Entities;

public class Enrollment
{
    public int Id { get; set; }

    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public int SessionId { get; set; }
    public Session Session { get; set; } = null!;

    public DateTime EnrollmentDateTime { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;
    public bool CancellationFeeApplied { get; set; } = false;

    // Relation with review - multiple reviews are possible for one enrollment
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

}