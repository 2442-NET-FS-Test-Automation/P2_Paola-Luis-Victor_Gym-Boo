namespace GymBoo.Data.Entities;

public class Availability
{
    public int Id { get; set; }

    public int InstructorId { get; set; }
    public Instructor Instructor { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; } // C# native enum(Sunday=0, Monday=1...)
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}