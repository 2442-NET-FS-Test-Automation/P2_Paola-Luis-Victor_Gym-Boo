using Gym_Boo.ControllerApi.Services;
using Gym_Boo.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gym_Boo.ControllerApi.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers data access with its lifecycles
    /// </summary>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // FIX: Evita que falle el comando 'ef migrations' si la cadena de conexión 
        // no está disponible temporalmente en las variables de entorno de las herramientas de diseño.
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = "Server=(localdb)\\mssqllocaldb;Database=GymBooDb_Design_Placeholder;Trusted_Connection=True;MultipleActiveResultSets=true";
        }

        // DbContext remains Scoped per HTTP request.
        // DbContextOptions are Singleton so they can be safely shared with
        // IDbContextFactory (registered as Singleton).

        services.AddDbContext<GymBooDbContext>(
        options =>
        {
            options.UseSqlServer(connectionString);
        },
        contextLifetime: ServiceLifetime.Scoped,
        optionsLifetime: ServiceLifetime.Singleton);

        services.AddDbContextFactory<GymBooDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }

    /// <summary>
    /// Here application services (business logic) are registered
    /// They are registered as Scoped, since they will depend on GymBooDbContext
    /// (also Scoped).
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IMemberRepository, MemberRepository>();

        return services;
    }
}