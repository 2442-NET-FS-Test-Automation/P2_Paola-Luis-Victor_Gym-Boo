namespace Gym_Boo.Data.Entities;

public class Place
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}