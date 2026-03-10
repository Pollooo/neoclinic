using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NeoClinic.Application.Common.Behaviours;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Application.Common.Services;
using NeoClinic.Application.Common.Services.TelegramBotService;
using NeoClinic.Application.Common.Services.TelegramBotService.UpdateHandler;
using System.Reflection;
using System.Text;
using Telegram.Bot;

namespace NeoClinic.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddSingleton<ITelegramBotClient>(sp =>
         {
             var config = sp.GetRequiredService<IConfiguration>();
             return new TelegramBotClient(config["Telegram:BotToken"]!);
         });
        services.AddSingleton<TelegramBotReceiver>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                ),
                ValidateLifetime = true
            };
        });

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminPolicy", policy =>
                policy.RequireRole("Admin"));

        //services.AddScoped<IStorageService, StorageService>();
        services.AddScoped<IStorageService, FirebaseStorageService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICommandHandler, CommandHandler>();
        services.AddScoped<ICallbackHandler, CallbackHandler>();

        return services;
    }
}
