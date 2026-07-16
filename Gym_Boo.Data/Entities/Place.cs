namespace GymBoo.Data.Entities;

public class Place
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public ICollection<Class> Classes { get; set; } = new List<Class>();
}