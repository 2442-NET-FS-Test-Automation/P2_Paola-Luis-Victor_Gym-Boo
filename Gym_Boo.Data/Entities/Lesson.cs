namespace GymBoo.Data.Entities;

public class Lesson
{
    public int Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Slots { get; set; } 

    public int ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public int InstructorId { get; set; }
    public Instructor Instructor { get; set; } = null!;
}