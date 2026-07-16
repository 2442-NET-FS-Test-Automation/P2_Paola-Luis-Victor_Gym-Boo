using Microsoft.EntityFrameworkCore;
using GymBoo.Data.Entities;
using GymBoo.Data.Enums;

public class GymBooDbContext : DbContext
{
    public GymBooDbContext(DbContextOptions<GymBooDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Instructor> Instructors => Set<Instructor>();

    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<MemberSubscription> MemberSubscriptions => Set<MemberSubscription>();

    public DbSet<Place> Places => Set<Place>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Lesson> Lessons => Set<Lesson>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

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
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.PasswordHash).IsRequired();

            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(sp => sp.Id);
            entity.Property(sp => sp.Name).IsRequired().HasMaxLength(100);
            entity.Property(sp => sp.Price).HasPrecision(18, 2);
            entity.Property(sp => sp.Recurrence).IsRequired();
        });

        modelBuilder.Entity<MemberSubscription>(entity =>
        {
            entity.HasKey(ms => ms.Id);
            entity.Property(ms => ms.StartDate).IsRequired();
            entity.Property(ms => ms.ExpirationDate).IsRequired();


            entity.HasOne(ms => ms.Plan)
                .WithMany(p => p.MemberSubscriptions)
                .HasForeignKey(ms => ms.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ms => ms.Member)
                .WithOne(m => m.MemberSubscription)
                .HasForeignKey<MemberSubscription>(ms => ms.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
            entity.Property(p => p.MaxCapacity).IsRequired();
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Description).HasMaxLength(500);
            entity.Property(c => c.DurationInMinutes).IsRequired();

            entity.HasOne(c => c.Place)
                .WithMany(p => p.Classes)
                .HasForeignKey(c => c.PlaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Start).IsRequired();
            entity.Property(l => l.End).IsRequired();
            entity.Property(l => l.Slots).IsRequired();

            entity.HasOne(l => l.Class)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(l => l.Instructor)
                .WithMany(i => i.Lessons)
                .HasForeignKey(l => l.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}