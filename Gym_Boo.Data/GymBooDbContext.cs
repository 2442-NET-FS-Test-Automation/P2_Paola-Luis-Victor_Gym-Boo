using Gym_Boo.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Gym_Boo.Data.Enums;
using Microsoft.AspNetCore.Identity;

public class GymBooDbContext : DbContext
{
    public GymBooDbContext(DbContextOptions<GymBooDbContext> options) : base(options)
    {
    }

    // --- USER HIERARCHY ---
    public DbSet<User> Users => Set<User>();
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
            .HasDiscriminator<Role>("Role")
            .HasValue<User>(Role.Admin) // Use 'Role' column for distinct them
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


        // --- SEEDING DATA ---

        // Hash estático precalculado de "Password123!" usando PasswordHasher<User>
        const string DEFAULT_PASSWORD_HASH = "AQAAAAIAAYagAAAAEBX4M2T5G6b9zQ8J7y/1v8K9R0P1Q2R3S4T5U6V7W8X9Y0Z==";

        // 1. USUARIOS (TPH)
        var adminUser = new User
        {
            Id = 1,
            Name = "Juan",
            LastName = "Admin",
            Email = "admin@gymboo.com",
            PasswordHash = DEFAULT_PASSWORD_HASH,
            Role = Role.Admin,
            IsActive = true
        };

        var instructor1 = new Instructor
        {
            Id = 2,
            Name = "Carlos",
            LastName = "Mendoza",
            Email = "carlos.mendoza@gymboo.com",
            PasswordHash = DEFAULT_PASSWORD_HASH,
            Role = Role.Instructor,
            IsActive = true
        };

        var instructor2 = new Instructor
        {
            Id = 3,
            Name = "Valeria",
            LastName = "Ríos",
            Email = "valeria.rios@gymboo.com",
            PasswordHash = DEFAULT_PASSWORD_HASH,
            Role = Role.Instructor,
            IsActive = true
        };

        var member1 = new Member
        {
            Id = 4,
            Name = "Sofía",
            LastName = "Gómez",
            Email = "sofia.gomez@gmail.com",
            PasswordHash = DEFAULT_PASSWORD_HASH,
            Role = Role.Member,
            IsActive = true
        };

        var member2 = new Member
        {
            Id = 5,
            Name = "Diego",
            LastName = "Torres",
            Email = "diego.torres@gmail.com",
            PasswordHash = DEFAULT_PASSWORD_HASH,
            Role = Role.Member,
            IsActive = true
        };

        modelBuilder.Entity<User>().HasData(adminUser);
        modelBuilder.Entity<Instructor>().HasData(instructor1, instructor2);
        modelBuilder.Entity<Member>().HasData(member1, member2);

        // 2. DISPONIBILIDADES DE INSTRUCTORES
        modelBuilder.Entity<Availability>().HasData(
            new Availability
            {
                Id = 1,
                InstructorId = 2, // Carlos Mendoza
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(7, 0, 0),
                EndTime = new TimeSpan(12, 0, 0)
            },
            new Availability
            {
                Id = 2,
                InstructorId = 3, // Valeria Ríos
                DayOfWeek = DayOfWeek.Wednesday,
                StartTime = new TimeSpan(16, 0, 0),
                EndTime = new TimeSpan(20, 0, 0)
            }
        );

        // 3. PLANES Y SUSCRIPCIONES
        var monthlyPlan = new SubscriptionPlan
        {
            Id = 1,
            Name = "Plan Mensual Estándar",
            Price = 799.00m,
            Recurrence = Recurrence.Monthly
        };

        var yearlyPlan = new SubscriptionPlan
        {
            Id = 2,
            Name = "Plan Anual VIP",
            Price = 7999.00m,
            Recurrence = Recurrence.Yearly
        };

        modelBuilder.Entity<SubscriptionPlan>().HasData(monthlyPlan, yearlyPlan);

        modelBuilder.Entity<MemberSubscription>().HasData(
            new MemberSubscription
            {
                Id = 1,
                MemberId = 4, // Sofía Gómez
                PlanId = 1,   // Mensual
                StartDate = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                ExpirationDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new MemberSubscription
            {
                Id = 2,
                MemberId = 5, // Diego Torres
                PlanId = 2,   // Anual
                StartDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ExpirationDate = new DateTime(2027, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // 4. LUGARES (PLACES)
        var mainHall = new Place { Id = 1, Name = "Salón Principal", MaxCapacity = 25 };
        var yogaStudio = new Place { Id = 2, Name = "Estudio Mind & Body", MaxCapacity = 15 };

        modelBuilder.Entity<Place>().HasData(mainHall, yogaStudio);

        // 5. DISCIPLINAS Y CLASES
        var crossfitDisc = new Discipline { Id = 1, Name = "CrossFit", Available = true };
        var yogaDisc = new Discipline { Id = 2, Name = "Yoga", Available = true };

        modelBuilder.Entity<Discipline>().HasData(crossfitDisc, yogaDisc);

        var crossfitBasic = new Class
        {
            Id = 1,
            Name = "CrossFit WOD Principiantes",
            Description = "Entrenamiento funcional de alta intensidad adaptado a nivel inicial.",
            DisciplineId = 1
        };

        var vinyasaYoga = new Class
        {
            Id = 2,
            Name = "Vinyasa Flow Yoga",
            Description = "Secuencia dinámica de posturas coordinadas con la respiración.",
            DisciplineId = 2
        };

        modelBuilder.Entity<Class>().HasData(crossfitBasic, vinyasaYoga);

        // 6. SESIONES
        var session1 = new Session
        {
            Id = 1,
            ClassId = 1,
            InstructorId = 2, // Carlos
            PlaceId = 1,      // Salón Principal
            Start = new DateTime(2026, 7, 21, 8, 0, 0, DateTimeKind.Utc),
            End = new DateTime(2026, 7, 21, 9, 0, 0, DateTimeKind.Utc),
            Slots = 20,
            CancellationFee = 50.00m
        };

        var session2 = new Session
        {
            Id = 2,
            ClassId = 2,
            InstructorId = 3, // Valeria
            PlaceId = 2,      // Estudio Mind & Body
            Start = new DateTime(2026, 7, 22, 17, 0, 0, DateTimeKind.Utc),
            End = new DateTime(2026, 7, 22, 18, 0, 0, DateTimeKind.Utc),
            Slots = 12,
            CancellationFee = 35.00m
        };

        modelBuilder.Entity<Session>().HasData(session1, session2);

        // 7. INSCRIPCIONES (ENROLLMENTS)
        var enrollment1 = new Enrollment
        {
            Id = 1,
            MemberId = 4, // Sofía
            SessionId = 1,
            EnrollmentDateTime = new DateTime(2026, 7, 20, 10, 30, 0, DateTimeKind.Utc),
            Status = EnrollmentStatus.Enrolled,
            CancellationFeeApplied = false
        };

        var enrollment2 = new Enrollment
        {
            Id = 2,
            MemberId = 5, // Diego
            SessionId = 2,
            EnrollmentDateTime = new DateTime(2026, 7, 20, 11, 15, 0, DateTimeKind.Utc),
            Status = EnrollmentStatus.Enrolled,
            CancellationFeeApplied = false
        };

        modelBuilder.Entity<Enrollment>().HasData(enrollment1, enrollment2);

        // 8. RESEÑAS
        modelBuilder.Entity<Review>().HasData(
            new Review
            {
                Id = 1,
                EnrollmentId = 1,
                SessionId = 1,
                ReviewType = ReviewType.Class,
                Rating = 5,
                Comment = "Excelente clase de CrossFit, muy dinámica y bien guiada.",
                CreatedAt = new DateTime(2026, 7, 21, 9, 15, 0, DateTimeKind.Utc)
            },
            new Review
            {
                Id = 2,
                EnrollmentId = 2,
                SessionId = 2,
                ReviewType = ReviewType.Instructor,
                Rating = 5,
                Comment = "Valeria explica los movimientos con mucha paciencia.",
                CreatedAt = new DateTime(2026, 7, 22, 18, 10, 0, DateTimeKind.Utc)
            }
        );
    }
}