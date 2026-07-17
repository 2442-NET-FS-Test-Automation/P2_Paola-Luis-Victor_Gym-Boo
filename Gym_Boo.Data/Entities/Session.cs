namespace GymBoo.Data.Entities;

public class Session
{
    public int Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Slots { get; set; } 

    public decimal CancellationFee { get; set; }

    public int ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public int InstructorId { get; set; }
    public Instructor Instructor { get; set; } = null!;

    public int PlaceId { get; set; }
    public Place Place { get; set; } = null!;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}