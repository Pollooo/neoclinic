using Microsoft.OpenApi;
using NeoClinic.Application.Common.Services.TelegramBotService.UpdateHandler;

namespace NeoClinic.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            const string schemeId = "Bearer";

            options.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference(schemeId, document),
                    new List<string>()
                }
            });
        });

        return services;
    }
}