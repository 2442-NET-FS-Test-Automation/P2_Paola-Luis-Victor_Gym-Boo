namespace Gym_Boo.Data.Entities;

public class Instructor : User
{
    public ICollection<Session> Sessions { get; set; } = new List<Session>();

    public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
}