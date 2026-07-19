namespace GymBoo.Data.Entities;

public class Discipline
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool Available { get; set; } = true;

    public ICollection<Class> Classes { get; set; } = new List<Class>();
}