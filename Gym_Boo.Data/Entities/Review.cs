using System.ComponentModel.DataAnnotations;

namespace GymBoo.Data.Entities;

public class Review
{
    public int Id { get; set; }

    public int EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; } = null!;

    public int SessionId { get; set; }
    public Session Session { get; set; } = null!;

    [Range(1, 5)]
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}