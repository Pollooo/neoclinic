using Microsoft.OpenApi;

namespace NeoClinic.Api;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSwaggerGenWithAuth()
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

        public IServiceCollection AddClinicCors()
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy
                        .WithOrigins(
                            "https://neoclinic-web-prod.azurewebsites.net",
                            "http://localhost:4200",
                            "http://uzclinic.runasp.net",
                            "https://localhost:7071",
                            "https://uzclinic.web.app"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return services;
        }
    }
}