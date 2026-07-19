using Microsoft.EntityFrameworkCore;
using GymBoo.Data.Entities;
using GymBoo.Data.Enums;

public class GymBooDbContext : DbContext
{
    public GymBooDbContext(DbContextOptions<GymBooDbContext> options) : base(options)
    {
    }

    // --- USER HIERARCHY ---
    public DbSet<User> Users => Set<User>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Instructor> Instructors => Set<Instructor>();

    // --- MEMBER'S PLANS AND SUBSCRIPTIONS  ---
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<MemberSubscription> MemberSubscriptions => Set<MemberSubscription>();

    // --- CLASSES/SESSIONS/ENROLLMENT OPERATIONS ---
    public DbSet<Place> Places => Set<Place>();
    public DbSet<Discipline> Disciplines => Set<Discipline>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    // --- FOR SPECIFIC FEATURES ---
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Availability> Availabilities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- Configuring data with Fluent API ---

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
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).IsRequired();

            entity.HasIndex(e => e.Email).IsUnique();

            //Global filter, EF will exclude inactive users automatically
            entity.HasQueryFilter(u => u.IsActive);
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

        modelBuilder.Entity<Discipline>(
            entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(30);
            entity.Property(p => p.Available).IsRequired().HasDefaultValue(true);

            entity.HasQueryFilter(d => d.Available);
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

            entity.HasOne(c => c.Discipline)
            .WithMany(d => d.Classes)
            .HasForeignKey(c => c.DisciplineId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Start).IsRequired();
            entity.Property(s => s.End).IsRequired();
            entity.Property(s => s.Slots).IsRequired();
            entity.Property(s => s.CancellationFee).IsRequired().HasPrecision(18,2);

            entity.HasOne(s => s.Class)
                .WithMany(s => s.Sessions)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.Instructor)
                .WithMany(s => s.Sessions)
                .HasForeignKey(s => s.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Place)
                .WithMany(p => p.Sessions)
                .HasForeignKey(s => s.PlaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).HasConversion<string>();

            entity.HasOne(e => e.Member)
                .WithMany(m => m.Enrollments)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Session)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Comment).HasMaxLength(1000);
            entity.Property(r => r.ReviewType).HasConversion<string>().IsRequired();

            entity.HasOne(r => r.Enrollment)
                .WithMany(e => e.Reviews)
                .HasForeignKey(r => r.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Session)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.SessionId)
                .OnDelete(DeleteBehavior.NoAction); // Solves cascade deletion on multiple paths for SQL Server
        });

        modelBuilder.Entity<Availability>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.HasOne(a => a.Instructor)
                .WithMany(i => i.Availabilities)
                .HasForeignKey(a => a.InstructorId)
                .OnDelete(DeleteBehavior.Cascade); //ON deleting instructor, also its availability
        });
    }
}