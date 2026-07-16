namespace GymBoo.Data.Entities;

public class Instructor : User
{
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}