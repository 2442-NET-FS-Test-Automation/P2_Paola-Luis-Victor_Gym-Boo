using Microsoft.EntityFrameworkCore;
using Gym_Boo.ControllerApi.Extensions;
using Gym_Boo.Controllers.Services;
using Gym_Boo.Controllers.Services.Interfaces;
using Serilog;
using Scalar.AspNetCore;
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

    // Adding CORS 
    const string SpaCorsPolicy = "spa"; // string name for our policy 

    // Configuring our CORS policy
    builder.Services.AddCors(o => o.AddPolicy(SpaCorsPolicy, p => p
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
    ));

    builder.Services.AddOpenApi();
    builder.Services.AddControllers();

    builder.Services.AddScoped<IAdminServices, AdminServices>();
    builder.Services.AddScoped<IInstructorServices, InstructorServices>();
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
        var db = scope.ServiceProvider.GetRequiredService<GymBooDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        const string password = "Password123!";

        async Task EnsureUserAsync(User user)
        {
            if (await db.Users.AnyAsync(x => x.Email == user.Email))
                return;

            user.PasswordHash = hasher.HashPassword(user, password);

            switch (user)
            {
                case Instructor instructor:
                    db.Instructors.Add(instructor);
                    break;

                case Member member:
                    db.Members.Add(member);
                    break;

                default:
                    db.Users.Add(user);
                    break;
            }
        }

        await EnsureUserAsync(new User
        {
            Name = "Michael",
            LastName = "Johnson",
            Email = "admin@gymboo.com",
            Role = Role.Admin,
            IsActive = true
        });

        await EnsureUserAsync(new Instructor
        {
            Name = "James",
            LastName = "Wilson",
            Email = "james.wilson@gymboo.com",
            Role = Role.Instructor,
            IsActive = true
        });

        await EnsureUserAsync(new Instructor
        {
            Name = "Emily",
            LastName = "Davis",
            Email = "emily.davis@gymboo.com",
            Role = Role.Instructor,
            IsActive = true
        });

        await EnsureUserAsync(new Member
        {
            Name = "Sarah",
            LastName = "Brown",
            Email = "sarah.brown@gmail.com",
            Role = Role.Member,
            IsActive = true
        });

        await EnsureUserAsync(new Member
        {
            Name = "Daniel",
            LastName = "Miller",
            Email = "daniel.miller@gmail.com",
            Role = Role.Member,
            IsActive = true
        });

        await db.SaveChangesAsync();
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
// FIX: Filtrar la excepción de aborto del host para que las herramientas de EF Core funcionen correctamente
catch (Exception e) when (e.GetType().Name != "HostAbortedException")
{
    Log.Fatal("The application terminated unexpectedly during startup: \n Message: {Message}", e.Message);
}
finally
{
    Log.CloseAndFlush();
}
