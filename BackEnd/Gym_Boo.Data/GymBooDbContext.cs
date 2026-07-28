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

        // =====================================================
        // 1. SUBSCRIPTION PLANS
        // =====================================================

        modelBuilder.Entity<SubscriptionPlan>().HasData(
            new SubscriptionPlan
            {
                Id = 1,
                Name = "Standard Monthly",
                Price = 49.99m,
                Recurrence = Recurrence.Monthly
            },
            new SubscriptionPlan
            {
                Id = 2,
                Name = "Premium Annual",
                Price = 499.99m,
                Recurrence = Recurrence.Yearly
            }
        );

        // =====================================================
        // 2. PLACES
        // =====================================================

        modelBuilder.Entity<Place>().HasData(

            new Place
            {
                Id = 1,
                Name = "Main Training Hall",
                MaxCapacity = 30
            },

            new Place
            {
                Id = 2,
                Name = "Mind & Body Studio",
                MaxCapacity = 20
            },

            new Place
            {
                Id = 3,
                Name = "Functional Zone",
                MaxCapacity = 18
            },

            new Place
            {
                Id = 4,
                Name = "Cycling Room",
                MaxCapacity = 25
            },

            new Place
            {
                Id = 5,
                Name = "Strength Room",
                MaxCapacity = 15
            }

        );

        // =====================================================
        // 3. DISCIPLINES
        // =====================================================

        modelBuilder.Entity<Discipline>().HasData(

            new Discipline
            {
                Id = 1,
                Name = "CrossFit",
                Available = true
            },

            new Discipline
            {
                Id = 2,
                Name = "Yoga",
                Available = true
            },

            new Discipline
            {
                Id = 3,
                Name = "HIIT",
                Available = true
            },

            new Discipline
            {
                Id = 4,
                Name = "Pilates",
                Available = true
            },

            new Discipline
            {
                Id = 5,
                Name = "Strength Training",
                Available = true
            },

            new Discipline
            {
                Id = 6,
                Name = "Indoor Cycling",
                Available = true
            }

        );

        // =====================================================
        // 4. CLASSES
        // =====================================================

        modelBuilder.Entity<Class>().HasData(

            // ---------- CrossFit ----------

            new Class
            {
                Id = 1,
                Name = "CrossFit Fundamentals",
                Description = "Introduction to functional movements and basic lifting techniques.",
                DisciplineId = 1
            },

            new Class
            {
                Id = 2,
                Name = "CrossFit Performance",
                Description = "High-intensity workout focused on strength and endurance.",
                DisciplineId = 1
            },

            // ---------- Yoga ----------

            new Class
            {
                Id = 3,
                Name = "Morning Flow Yoga",
                Description = "A relaxing flow to improve flexibility and mobility.",
                DisciplineId = 2
            },

            new Class
            {
                Id = 4,
                Name = "Power Yoga",
                Description = "Dynamic yoga practice designed to build strength and balance.",
                DisciplineId = 2
            },

            // ---------- HIIT ----------

            new Class
            {
                Id = 5,
                Name = "HIIT Express",
                Description = "Fast-paced interval workout completed in 30 minutes.",
                DisciplineId = 3
            },

            new Class
            {
                Id = 6,
                Name = "HIIT Burn",
                Description = "Full-body interval training with maximum calorie burn.",
                DisciplineId = 3
            },

            // ---------- Pilates ----------

            new Class
            {
                Id = 7,
                Name = "Pilates Core",
                Description = "Improve posture, stability, and core strength.",
                DisciplineId = 4
            },

            new Class
            {
                Id = 8,
                Name = "Pilates Balance",
                Description = "Low-impact class focused on coordination and flexibility.",
                DisciplineId = 4
            },

            // ---------- Strength ----------

            new Class
            {
                Id = 9,
                Name = "Upper Body Strength",
                Description = "Resistance training focused on chest, shoulders, back, and arms.",
                DisciplineId = 5
            },

            new Class
            {
                Id = 10,
                Name = "Lower Body Strength",
                Description = "Strength session emphasizing legs, glutes, and core stability.",
                DisciplineId = 5
            },

            // ---------- Cycling ----------

            new Class
            {
                Id = 11,
                Name = "Beginner Cycling",
                Description = "Indoor cycling session for new riders.",
                DisciplineId = 6
            },

            new Class
            {
                Id = 12,
                Name = "Endurance Ride",
                Description = "Long-distance cycling workout with progressive intensity.",
                DisciplineId = 6
            }

        );

        // =====================================================
        // 5. INSTRUCTOR AVAILABILITY
        // =====================================================

        modelBuilder.Entity<Availability>().HasData(

            // James Wilson

            new Availability
            {
                Id = 1,
                InstructorId = 2,
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(7, 0, 0),
                EndTime = new TimeSpan(12, 0, 0)
            },

            new Availability
            {
                Id = 2,
                InstructorId = 2,
                DayOfWeek = DayOfWeek.Wednesday,
                StartTime = new TimeSpan(7, 0, 0),
                EndTime = new TimeSpan(12, 0, 0)
            },

            new Availability
            {
                Id = 3,
                InstructorId = 2,
                DayOfWeek = DayOfWeek.Friday,
                StartTime = new TimeSpan(7, 0, 0),
                EndTime = new TimeSpan(12, 0, 0)
            },

            // Emily Davis

            new Availability
            {
                Id = 4,
                InstructorId = 3,
                DayOfWeek = DayOfWeek.Tuesday,
                StartTime = new TimeSpan(16, 0, 0),
                EndTime = new TimeSpan(20, 0, 0)
            },

            new Availability
            {
                Id = 5,
                InstructorId = 3,
                DayOfWeek = DayOfWeek.Thursday,
                StartTime = new TimeSpan(16, 0, 0),
                EndTime = new TimeSpan(20, 0, 0)
            },

            new Availability
            {
                Id = 6,
                InstructorId = 3,
                DayOfWeek = DayOfWeek.Saturday,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(13, 0, 0)
            }

        );



        // =====================================================
        // 6. SESSIONS
        // =====================================================

        modelBuilder.Entity<Session>().HasData(

            // ---------- PAST ----------

            new Session
            {
                Id = 1,
                ClassId = 1,
                InstructorId = 2,
                PlaceId = 1,
                Start = new DateTime(2026, 5, 4, 12, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 5, 4, 13, 0, 0, DateTimeKind.Utc),
                Slots = 20,
                CancellationFee = 15
            },

            new Session
            {
                Id = 2,
                ClassId = 3,
                InstructorId = 1,
                PlaceId = 2,
                Start = new DateTime(2026, 5, 5, 17, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 5, 5, 18, 0, 0, DateTimeKind.Utc),
                Slots = 18,
                CancellationFee = 10
            },

            new Session
            {
                Id = 3,
                ClassId = 5,
                InstructorId = 1,
                PlaceId = 3,
                Start = new DateTime(2026, 5, 20, 13, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 5, 20, 14, 0, 0, DateTimeKind.Utc),
                Slots = 18,
                CancellationFee = 15
            },

            new Session
            {
                Id = 4,
                ClassId = 7,
                InstructorId = 1,
                PlaceId = 2,
                Start = new DateTime(2026, 6, 2, 17, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 6, 2, 18, 0, 0, DateTimeKind.Utc),
                Slots = 15,
                CancellationFee = 10
            },

            new Session
            {
                Id = 5,
                ClassId = 9,
                InstructorId = 2,
                PlaceId = 5,
                Start = new DateTime(2026, 6, 10, 18, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 6, 10, 19, 0, 0, DateTimeKind.Utc),
                Slots = 15,
                CancellationFee = 15
            },

            new Session
            {
                Id = 6,
                ClassId = 11,
                InstructorId = 1,
                PlaceId = 4,
                Start = new DateTime(2026, 6, 13, 19, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 6, 13, 20, 0, 0, DateTimeKind.Utc),
                Slots = 20,
                CancellationFee = 10
            },

            new Session
            {
                Id = 7,
                ClassId = 2,
                InstructorId = 2,
                PlaceId = 1,
                Start = new DateTime(2026, 6, 24, 18, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 6, 24, 19, 0, 0, DateTimeKind.Utc),
                Slots = 25,
                CancellationFee = 15
            },

            new Session
            {
                Id = 8,
                ClassId = 4,
                InstructorId = 1,
                PlaceId = 2,
                Start = new DateTime(2026, 7, 2, 17, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 7, 2, 18, 0, 0, DateTimeKind.Utc),
                Slots = 18,
                CancellationFee = 10
            },

            // ---------- RECENT ----------

            new Session
            {
                Id = 9,
                ClassId = 6,
                InstructorId = 2,
                PlaceId = 3,
                Start = new DateTime(2026, 7, 15, 19, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 7, 15, 20, 0, 0, DateTimeKind.Utc),
                Slots = 18,
                CancellationFee = 15
            },

            new Session
            {
                Id = 10,
                ClassId = 8,
                InstructorId = 1,
                PlaceId = 2,
                Start = new DateTime(2026, 7, 18, 17, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 7, 18, 18, 0, 0, DateTimeKind.Utc),
                Slots = 15,
                CancellationFee = 10
            },

            // ---------- UPCOMING ----------

            new Session
            {
                Id = 11,
                ClassId = 1,
                InstructorId = 2,
                PlaceId = 1,
                Start = new DateTime(2026, 7, 29, 18, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 7, 29, 19, 0, 0, DateTimeKind.Utc),
                Slots = 20,
                CancellationFee = 15
            },

            new Session
            {
                Id = 12,
                ClassId = 3,
                InstructorId = 1,
                PlaceId = 2,
                Start = new DateTime(2026, 7, 30, 17, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 7, 30, 18, 0, 0, DateTimeKind.Utc),
                Slots = 18,
                CancellationFee = 10
            },

            new Session
            {
                Id = 13,
                ClassId = 10,
                InstructorId = 1,
                PlaceId = 5,
                Start = new DateTime(2026, 8, 3, 21, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 8, 3, 22, 0, 0, DateTimeKind.Utc),
                Slots = 15,
                CancellationFee = 15
            },

            new Session
            {
                Id = 14,
                ClassId = 12,
                InstructorId = 1,
                PlaceId = 4,
                Start = new DateTime(2026, 8, 4, 18, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 8, 4, 19, 0, 0, DateTimeKind.Utc),
                Slots = 20,
                CancellationFee = 10
            },

            new Session
            {
                Id = 15,
                ClassId = 5,
                InstructorId = 2,
                PlaceId = 3,
                Start = new DateTime(2026, 8, 12, 22, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 8, 12, 23, 0, 0, DateTimeKind.Utc),
                Slots = 18,
                CancellationFee = 15
            },

            new Session
            {
                Id = 16,
                ClassId = 7,
                InstructorId = 1,
                PlaceId = 2,
                Start = new DateTime(2026, 8, 13, 17, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 8, 13, 18, 0, 0, DateTimeKind.Utc),
                Slots = 15,
                CancellationFee = 10
            },

            // ---------- FAR FUTURE ----------

            new Session
            {
                Id = 17,
                ClassId = 2,
                InstructorId = 2,
                PlaceId = 1,
                Start = new DateTime(2026, 9, 9, 18, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 9, 9, 19, 0, 0, DateTimeKind.Utc),
                Slots = 25,
                CancellationFee = 15
            },

            new Session
            {
                Id = 18,
                ClassId = 4,
                InstructorId = 1,
                PlaceId = 2,
                Start = new DateTime(2026, 9, 10, 17, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 9, 10, 18, 0, 0, DateTimeKind.Utc),
                Slots = 18,
                CancellationFee = 10
            },

            new Session
            {
                Id = 19,
                ClassId = 9,
                InstructorId = 2,
                PlaceId = 5,
                Start = new DateTime(2026, 10, 5, 15, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 10, 5, 16, 0, 0, DateTimeKind.Utc),
                Slots = 15,
                CancellationFee = 15
            },

            new Session
            {
                Id = 20,
                ClassId = 11,
                InstructorId = 1,
                PlaceId = 4,
                Start = new DateTime(2026, 10, 6, 18, 0, 0, DateTimeKind.Utc),
                End = new DateTime(2026, 10, 6, 19, 0, 0, DateTimeKind.Utc),
                Slots = 20,
                CancellationFee = 10
            }

        );

        // =====================================================
        // 7. MEMBER SUBSCRIPTIONS
        // =====================================================

        modelBuilder.Entity<MemberSubscription>().HasData(

            new MemberSubscription
            {
                Id = 1,
                MemberId = 4,
                PlanId = 1,
                StartDate = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                ExpirationDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)
            },

            new MemberSubscription
            {
                Id = 2,
                MemberId = 5,
                PlanId = 2,
                StartDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ExpirationDate = new DateTime(2027, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }

        );

        // =====================================================
        // 8. ENROLLMENTS
        // =====================================================

        modelBuilder.Entity<Enrollment>().HasData(

            // =================================================
            // SARAH BROWN
            // =================================================

            new Enrollment
            {
                Id = 1,
                MemberId = 4,
                SessionId = 1,
                EnrollmentDateTime = new DateTime(2026, 5, 2, 12, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Attended,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 2,
                MemberId = 4,
                SessionId = 3,
                EnrollmentDateTime = new DateTime(2026, 5, 18, 9, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Attended,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 3,
                MemberId = 4,
                SessionId = 5,
                EnrollmentDateTime = new DateTime(2026, 6, 8, 14, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Attended,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 4,
                MemberId = 4,
                SessionId = 7,
                EnrollmentDateTime = new DateTime(2026, 6, 22, 10, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Attended,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 5,
                MemberId = 4,
                SessionId = 9,
                EnrollmentDateTime = new DateTime(2026, 7, 13, 13, 30, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Attended,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 6,
                MemberId = 4,
                SessionId = 11,
                EnrollmentDateTime = new DateTime(2026, 7, 25, 10, 15, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Enrolled,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 7,
                MemberId = 4,
                SessionId = 13,
                EnrollmentDateTime = new DateTime(2026, 7, 28, 15, 20, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Enrolled,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 8,
                MemberId = 4,
                SessionId = 17,
                EnrollmentDateTime = new DateTime(2026, 8, 20, 9, 15, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Enrolled,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 9,
                MemberId = 4,
                SessionId = 20,
                EnrollmentDateTime = new DateTime(2026, 9, 20, 11, 15, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Enrolled,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 10,
                MemberId = 4,
                SessionId = 8,
                EnrollmentDateTime = new DateTime(2026, 6, 29, 9, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Cancelled,
                CancellationFeeApplied = true
            },



            // =================================================
            // DANIEL MILLER
            // =================================================

            new Enrollment
            {
                Id = 11,
                MemberId = 5,
                SessionId = 2,
                EnrollmentDateTime = new DateTime(2026, 5, 3, 11, 30, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Cancelled,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 12,
                MemberId = 5,
                SessionId = 4,
                EnrollmentDateTime = new DateTime(2026, 5, 30, 8, 45, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Attended,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 13,
                MemberId = 5,
                SessionId = 6,
                EnrollmentDateTime = new DateTime(2026, 6, 10, 12, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Attended,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 14,
                MemberId = 5,
                SessionId = 10,
                EnrollmentDateTime = new DateTime(2026, 7, 16, 13, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Attended,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 15,
                MemberId = 5,
                SessionId = 12,
                EnrollmentDateTime = new DateTime(2026, 7, 26, 10, 30, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Attended,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 16,
                MemberId = 5,
                SessionId = 14,
                EnrollmentDateTime = new DateTime(2026, 8, 1, 14, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Attended,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 17,
                MemberId = 5,
                SessionId = 18,
                EnrollmentDateTime = new DateTime(2026, 8, 25, 10, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Enrolled,
                CancellationFeeApplied = false
            },

            new Enrollment
            {
                Id = 18,
                MemberId = 5,
                SessionId = 19,
                EnrollmentDateTime = new DateTime(2026, 9, 25, 16, 0, 0, DateTimeKind.Utc),
                Status = EnrollmentStatus.Attended,
                CancellationFeeApplied = false
            }

        );

        // =====================================================
        // 9. REVIEWS
        // =====================================================

        modelBuilder.Entity<Review>().HasData(

            // =================================================
            // SARAH BROWN
            // =================================================

            new Review
            {
                Id = 1,
                EnrollmentId = 1,
                SessionId = 1,
                ReviewType = ReviewType.Class,
                Rating = 5,
                Comment = "Excellent introduction to CrossFit. The class was engaging and well structured.",
                CreatedAt = new DateTime(2026, 5, 4, 9, 15, 0, DateTimeKind.Utc)
            },

            new Review
            {
                Id = 2,
                EnrollmentId = 1,
                SessionId = 1,
                ReviewType = ReviewType.Instructor,
                Rating = 5,
                Comment = "James was motivating and explained every exercise clearly.",
                CreatedAt = new DateTime(2026, 5, 4, 9, 16, 0, DateTimeKind.Utc)
            },

            new Review
            {
                Id = 3,
                EnrollmentId = 2,
                SessionId = 3,
                ReviewType = ReviewType.Class,
                Rating = 4,
                Comment = "Great workout with a good balance between cardio and strength.",
                CreatedAt = new DateTime(2026, 5, 20, 10, 10, 0, DateTimeKind.Utc)
            },

            new Review
            {
                Id = 4,
                EnrollmentId = 3,
                SessionId = 5,
                ReviewType = ReviewType.Instructor,
                Rating = 5,
                Comment = "Excellent coaching and constant feedback during the exercises.",
                CreatedAt = new DateTime(2026, 6, 10, 9, 20, 0, DateTimeKind.Utc)
            },

            new Review
            {
                Id = 5,
                EnrollmentId = 4,
                SessionId = 7,
                ReviewType = ReviewType.Class,
                Rating = 5,
                Comment = "One of my favorite CrossFit classes so far.",
                CreatedAt = new DateTime(2026, 6, 24, 9, 15, 0, DateTimeKind.Utc)
            },

            // Sarah's Session 9 intentionally has NO review
            // to demonstrate the "Leave Review" feature.

            // =================================================
            // DANIEL MILLER
            // =================================================

            new Review
            {
                Id = 6,
                EnrollmentId = 11,
                SessionId = 2,
                ReviewType = ReviewType.Class,
                Rating = 5,
                Comment = "Very relaxing yoga session after work.",
                CreatedAt = new DateTime(2026, 5, 5, 18, 10, 0, DateTimeKind.Utc)
            },

            new Review
            {
                Id = 7,
                EnrollmentId = 11,
                SessionId = 2,
                ReviewType = ReviewType.Instructor,
                Rating = 5,
                Comment = "Emily creates a welcoming environment for everyone.",
                CreatedAt = new DateTime(2026, 5, 5, 18, 11, 0, DateTimeKind.Utc)
            },

            new Review
            {
                Id = 8,
                EnrollmentId = 12,
                SessionId = 4,
                ReviewType = ReviewType.Class,
                Rating = 4,
                Comment = "Great Pilates class focused on core stability.",
                CreatedAt = new DateTime(2026, 6, 2, 18, 10, 0, DateTimeKind.Utc)
            },

            new Review
            {
                Id = 9,
                EnrollmentId = 13,
                SessionId = 6,
                ReviewType = ReviewType.Class,
                Rating = 5,
                Comment = "The cycling workout was challenging and fun.",
                CreatedAt = new DateTime(2026, 6, 13, 10, 15, 0, DateTimeKind.Utc)
            },

            new Review
            {
                Id = 10,
                EnrollmentId = 13,
                SessionId = 6,
                ReviewType = ReviewType.Instructor,
                Rating = 4,
                Comment = "Emily kept everyone motivated throughout the session.",
                CreatedAt = new DateTime(2026, 6, 13, 10, 16, 0, DateTimeKind.Utc)
            },

            // Daniel's Session 10 intentionally has NO review.

            // =================================================
            // EXTRA REVIEWS
            // =================================================

            new Review
            {
                Id = 11,
                EnrollmentId = 2,
                SessionId = 3,
                ReviewType = ReviewType.Instructor,
                Rating = 4,
                Comment = "James maintained a great pace during the HIIT session.",
                CreatedAt = new DateTime(2026, 5, 20, 10, 11, 0, DateTimeKind.Utc)
            },

            new Review
            {
                Id = 12,
                EnrollmentId = 12,
                SessionId = 4,
                ReviewType = ReviewType.Instructor,
                Rating = 5,
                Comment = "Emily pays close attention to everyone's technique.",
                CreatedAt = new DateTime(2026, 6, 2, 18, 11, 0, DateTimeKind.Utc)
            }

        );
    
    }
}