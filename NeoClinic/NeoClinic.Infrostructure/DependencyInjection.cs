using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Entities;
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

            if (!dbContext.Database.CanConnect())
            {
                Console.WriteLine("⚠️ Could not connect to database — migrations skipped");
                return;
            }

            dbContext.Database.Migrate();
            Console.WriteLine("✅ Database migrations applied successfully");

            ImportLegacyMappings(dbContext);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Database migration failed: {ex.Message}");
            Console.WriteLine("The API will continue to run, but database features may be unavailable.");
            Console.WriteLine("Run 'dotnet ef database update' manually after fixing connection issues.");
        }
    }

    private static void ImportLegacyMappings(ApplicationDbContext dbContext)
    {
        try
        {
            var mappingFilePath = Path.Combine(AppContext.BaseDirectory, "telegram-mappings.json");
            if (!File.Exists(mappingFilePath))
                return;

            var hasExistingData = dbContext.TelegramFileMaps.Any();
            if (hasExistingData)
            {
                Console.WriteLine("ℹ️ TelegramFileMaps table already has data — skipping legacy import");
                return;
            }

            var json = File.ReadAllText(mappingFilePath);
            var legacyMappings = JsonSerializer.Deserialize<Dictionary<string, LegacyFileMapping>>(json);
            if (legacyMappings is null || legacyMappings.Count == 0)
                return;

            var count = 0;
            foreach (var (blobName, mapping) in legacyMappings)
            {
                dbContext.TelegramFileMaps.Add(new TelegramFileMap
                {
                    BlobName = blobName,
                    FileId = mapping.FileId,
                    ContentType = mapping.ContentType,
                    CreatedAt = DateTime.UtcNow
                });
                count++;
            }
            dbContext.SaveChanges();
            Console.WriteLine($"✅ Imported {count} legacy Telegram file mappings to database");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Failed to import legacy Telegram mappings: {ex.Message}");
        }
    }

    private class LegacyFileMapping
    {
        public string FileId { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
