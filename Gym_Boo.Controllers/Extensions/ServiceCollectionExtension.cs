using Microsoft.EntityFrameworkCore;

namespace Gymboo.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers data access with its lifecycles
    /// </summary>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection'not found.");

        // DbContext remains Scoped per HTTP request.
        // DbContextOptions are Singleton so they can be safely shared with
        // IDbContextFactory (registered as Singleton).
        
        // services.AddDbContext<GymbooDbContext>(
        // options =>
        // {
        //     options.UseSqlServer(connectionString);
        // },
        // contextLifetime: ServiceLifetime.Scoped,
        // optionsLifetime: ServiceLifetime.Singleton);

        // services.AddDbContextFactory<GymbooDbContext>(options =>
        // {
        //     options.UseSqlServer(connectionString);
        // });

        return services;
    }

    /// <summary>
    /// Here application services (business logic) are registered
    /// They are registered as Scoped, since they will depend on PharmacyDbContext
    /// (also Scoped).
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // services.AddScoped<IFulfillmentService, FulfillmentService>();
        // services.AddScoped<OrderFactory>();
        // services.AddScoped<BurstPlanner>();
        // services.AddScoped<ISeeder, Seeder>();

        return services;
    }
}