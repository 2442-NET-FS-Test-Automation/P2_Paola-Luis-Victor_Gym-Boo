using Microsoft.EntityFrameworkCore;
using Gym_Boo.Controllers.Services;
using Serilog;
using Scalar.AspNetCore;
using Gym_Boo.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Gym_Boo.ControllerApi.Extensions;
using Gym_Boo.Controllers.DTOs;

// Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/fulfillment-log-.log",
        rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting GymBoo API");

    var builder = WebApplication.CreateBuilder();

    // ---------- BUILDER: SERVICES REGISTRY ----------
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    const string SpaCorsPolicy = "spa";

    builder.Services.AddCors(o => o.AddPolicy(SpaCorsPolicy, p => p
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
    ));

    builder.Services.AddOpenApi();
    builder.Services.AddControllers()
        .AddApplicationPart(typeof(Gym_Boo.Controllers.Controllers.AuthController).Assembly);
    
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    
    builder.Services.AddPersistence(builder.Configuration, builder.Environment.IsEnvironment("Testing"));
    builder.Services.AddApplicationServices();

    string jwtKey = builder.Configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("JWT key is missing.");

    string jwtIssuer = builder.Configuration["Jwt:Issuer"]
        ?? throw new InvalidOperationException("JWT issuer is missing.");

    string jwtAudience = builder.Configuration["Jwt:Audience"]
        ?? throw new InvalidOperationException("JWT audience is missing.");

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    // ---------- SEED DATABASE FROM JSON ----------
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<GymBooDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        await SeedDatabaseFromJsonAsync(db, hasher);
    }
    
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseCors(SpaCorsPolicy);

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapGet("/", () => Results.Redirect("Scalar/#tag/gym-boocontrollers"));

    app.Run();
}
catch (Exception e) when (e.GetType().Name != "HostAbortedException")
{
    Log.Fatal("The application terminated unexpectedly during startup: \n Message: {Message}", e.Message);
}
finally
{
    Log.CloseAndFlush();
}

// ---------- SEEDING METHOD & DTOS ----------
public partial class Program 
{
    private static async Task SeedDatabaseFromJsonAsync(GymBooDbContext db, IPasswordHasher<User> hasher)
    {
        if (await db.Users.AnyAsync()) return;

        string filePath = Path.Combine(AppContext.BaseDirectory,"Seed", "gymboo-seed-data.json");
        if (!File.Exists(filePath)) return;

        string json = await File.ReadAllTextAsync(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        var data = JsonSerializer.Deserialize<SeedDataRoot>(json, options);
        if (data == null) return;

        // 1. Users, places and disciplines
        var userEntities = new List<User>();
        foreach (var dto in data.Users)
        {
            User user = dto.Type switch
            {
                "Instructor" => new Instructor(),
                "Member" => new Member(),
                _ => new User()
            };

            user.Name = dto.Name;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.Role = dto.Role;
            user.IsActive = dto.IsActive;
            var seedPassword = dto.SeedPasswordKey == "GYMBOO_DEFAULT_PASSWORD"
                ? "Password123!"
                : dto.SeedPasswordKey;

            user.PasswordHash = hasher.HashPassword(user, seedPassword);

            userEntities.Add(user);
        }

        db.Users.AddRange(userEntities);
        db.SubscriptionPlans.AddRange(data.SubscriptionPlans);
        db.Places.AddRange(data.Places);
        db.Disciplines.AddRange(data.Disciplines);

        await db.SaveChangesAsync();

        // 2. Classes & Availabilities
        var classEntities = data.Classes.Select(c => new Class
        {
            Name = c.Name,
            Description = c.Description,
            DisciplineId = data.Disciplines[c.DisciplineId - 1].Id
        }).ToList();
        db.Classes.AddRange(classEntities);

        var availabilityEntities = data.Availabilities.Select(a => new Availability
        {
            InstructorId = userEntities[a.InstructorId - 1].Id,
            DayOfWeek = a.DayOfWeek,
            StartTime = a.StartTime,
            EndTime = a.EndTime
        }).ToList();
        db.Availabilities.AddRange(availabilityEntities);

        await db.SaveChangesAsync();

        // 3. Sessions & Subscriptions
        var sessionEntities = data.Sessions.Select(s => new Session
        {
            ClassId = classEntities[s.ClassId - 1].Id,
            InstructorId = userEntities[s.InstructorId - 1].Id,
            PlaceId = data.Places[s.PlaceId - 1].Id,
            Start = s.Start,
            End = s.End,
            Slots = s.Slots,
            CancellationFee = s.CancellationFee
        }).ToList();
        db.Sessions.AddRange(sessionEntities);

        var subscriptionEntities = data.MemberSubscriptions.Select(ms => new MemberSubscription
        {
            MemberId = userEntities[ms.MemberId - 1].Id,
            PlanId = data.SubscriptionPlans[ms.PlanId - 1].Id,
            StartDate = ms.StartDate,
            ExpirationDate = ms.ExpirationDate
        }).ToList();
        db.MemberSubscriptions.AddRange(subscriptionEntities);

        await db.SaveChangesAsync();

        // 4. Enrollments
        var enrollmentEntities = data.Enrollments.Select(e => new Enrollment
        {
            MemberId = userEntities[e.MemberId - 1].Id,
            SessionId = sessionEntities[e.SessionId - 1].Id,
            EnrollmentDateTime = e.EnrollmentDateTime,
            Status = e.Status,
            CancellationFeeApplied = e.CancellationFeeApplied
        }).ToList();
        db.Enrollments.AddRange(enrollmentEntities);

        await db.SaveChangesAsync();

        // 5. Reviews
        var reviewEntities = data.Reviews.Select(r => new Review
        {
            EnrollmentId = enrollmentEntities[r.EnrollmentId - 1].Id,
            SessionId = sessionEntities[r.SessionId - 1].Id,
            ReviewType = r.ReviewType,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        }).ToList();
        db.Reviews.AddRange(reviewEntities);

        await db.SaveChangesAsync();
    }
}
