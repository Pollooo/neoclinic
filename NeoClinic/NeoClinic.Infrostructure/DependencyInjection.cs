using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Infrostructure.Persistance;

namespace NeoClinic.Infrostructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.WriteLine("⚠️ Connection string 'Database' is missing. Add it to appsettings.json or environment variables.");
            Console.WriteLine("   Example (appsettings.json): \"ConnectionStrings\": { \"Database\": \"Server=...;Database=...;User Id=...;Password=...;\" }");
            Console.WriteLine("   Example (env var): ConnectionStrings__Database=Server=...");
        }

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        return services;
    }

    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        try
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using ApplicationDbContext dbContext =
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (dbContext.Database.CanConnect())
            {
                dbContext.Database.Migrate();
                Console.WriteLine("✅ Database migrations applied successfully");
            }
            else
            {
                Console.WriteLine("⚠️ Could not connect to database — migrations skipped");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Database migration failed: {ex.Message}");
            Console.WriteLine("The API will continue to run, but database features may be unavailable.");
            Console.WriteLine("Run 'dotnet ef database update' manually after fixing connection issues.");
        }
    }
}
