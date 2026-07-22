using System.Diagnostics;
using Gym_Boo.Controllers.Administration;
using Microsoft.EntityFrameworkCore;
using Gym_Boo.ControllerApi.Extensions;
using Serilog;
using Scalar.AspNetCore;
using Gym_Boo.Controllers.Services;
using Gym_Boo.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Gym_Boo.Data.Enums;

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
    // Replace the default logger provider with Serilog
    // ReadFrom.Configuration takes "Serilog" section from appsettings.json
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddOpenApi();
    builder.Services.AddControllers();

    builder.Services.AddScoped<IAdminServices, AdminServices>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IPasswordHasher<User>,PasswordHasher<User>>();

    

    // Persistency
    // DbContext (Scoped) + IDbContextFactory(Singleton) 
    // For concurrent operations (if applies)
    builder.Services.AddPersistence(builder.Configuration);

    // Application Services
    builder.Services.AddApplicationServices();

    ///////////////
    string jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT key is missing.");

    string jwtIssuer = builder.Configuration["Jwt:Issuer"]
        ?? throw new InvalidOperationException(
            "JWT issuer is missing.");

    string jwtAudience = builder.Configuration["Jwt:Audience"]
        ?? throw new InvalidOperationException(
            "JWT audience is missing.");

    builder.Services
        .AddAuthentication(
            JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey)),

                    ClockSkew = TimeSpan.Zero
                };
        });

    builder.Services.AddAuthorization();
    /// 

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider
            .GetRequiredService<GymBooDbContext>();

        var hasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher<User>>();

        const string defaultPassword = "Password123!";

        // NEW ADMIN
        const string adminEmail = "mariana.lopez@gymboo.com";

        if (!await db.Users.AnyAsync(u => u.Email == adminEmail))
        {
            var admin = new User
            {
                Name = "Mariana",
                LastName = "López",
                Email = adminEmail,
                Role = Role.Admin,
                IsActive = true
            };

            admin.PasswordHash =
                hasher.HashPassword(admin, defaultPassword);

            db.Users.Add(admin);
        }

        // NEW INSTRUCTOR
        const string instructorEmail = "andres.ramirez@gymboo.com";

        if (!await db.Users.AnyAsync(u => u.Email == instructorEmail))
        {
            var instructor = new Instructor
            {
                Name = "Andrés",
                LastName = "Ramírez",
                Email = instructorEmail,
                Role = Role.Instructor,
                IsActive = true
            };

            instructor.PasswordHash =
                hasher.HashPassword(instructor, defaultPassword);

            db.Instructors.Add(instructor);
        }

        // NEW MEMBER
        const string memberEmail = "camila.hernandez@gmail.com";

        if (!await db.Users.AnyAsync(u => u.Email == memberEmail))
        {
            var member = new Member
            {
                Name = "Camila",
                LastName = "Hernández",
                Email = memberEmail,
                Role = Role.Member,
                IsActive = true
            };

            member.PasswordHash =
                hasher.HashPassword(member, defaultPassword);

            db.Members.Add(member);
        }

        await db.SaveChangesAsync();
    }

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseAuthentication();
    app.UseAuthorization(); 

    app.MapControllers();

    app.MapGet("/", () => Results.Redirect("Scalar/#tag/gym-boocontrollers"));

    app.Run();
}
// FIX: Filtrar la excepción de aborto del host para que las herramientas de EF Core funcionen correctamente
catch (Exception e) when (e.GetType().Name != "HostAbortedException")
{
    Log.Fatal("The application terminated unexpectedly during startup: \n Message: {Message}", e.Message);
}
finally
{
    Log.CloseAndFlush();
}
