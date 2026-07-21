namespace Gym_Boo.Data.Entities;

public class Class
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Session> Sessions { get; set; } = new List<Session>();

    public int DisciplineId { get; set; }
    public Discipline Discipline { get; set; } = null!;
}