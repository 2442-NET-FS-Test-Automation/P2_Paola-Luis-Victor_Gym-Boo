using System.Diagnostics;
using Gym_Boo.Controllers.Administration;
using Microsoft.EntityFrameworkCore;
using Gym_Boo.ControllerApi.Extensions;
using Serilog;
using Scalar.AspNetCore;

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

    // Persistency
    // DbContext (Scoped) + IDbContextFactory(Singleton) 
    // For concurrent operations (if applies)
    builder.Services.AddPersistence(builder.Configuration);

    // Application Services
    builder.Services.AddApplicationServices();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseCors(SpaCorsPolicy);

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
