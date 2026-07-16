using Microsoft.EntityFrameworkCore;
using GymBoo.Data.Entities;
using GymBoo.Data.Enums;

public class GymBooDbContext : DbContext
{
    public GymBooDbContext(DbContextOptions<GymBooDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Instructor> Instructors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Inheritance config TPH (Table-Per-Hierarchy)
        modelBuilder.Entity<User>()
            .HasDiscriminator<Role>("Role") // Use 'Role' column for distinct them
            .HasValue<Admin>(Role.Admin)
            .HasValue<Member>(Role.Member)
            .HasValue<Instructor>(Role.Instructor);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Lastname).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.PasswordHash).IsRequired();

            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}