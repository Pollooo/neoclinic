using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NeoClinic.Application.Common.Behaviours;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Application.Common.Services;
using NeoClinic.Application.Common.Services.TelegramBotService;
using System.Reflection;

namespace NeoClinic.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminPolicy", policy =>
                policy.RequireRole("Admin"));

        services.AddScoped<IStorageService, StorageService>();
        services.AddScoped<ITelegramBotService, TelegramBotService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
