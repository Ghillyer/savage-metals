using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Repositories;
using SavageMetals.Api.DAL;
using SavageMetals.Api.Factories;
using SavageMetals.Api.Middleware;
using SavageMetals.Api.Security;
using SavageMetals.Api.Services;
using SavageMetals.Api.Settings;

namespace SavageMetals.Api;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSavageMetalsServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Error handling
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // Authentication
        var firebaseSettings = configuration.GetSection(FirebaseSettings.SectionName).Get<FirebaseSettings>()
            ?? throw new InvalidOperationException("Firebase settings are not configured.");

        if (string.IsNullOrWhiteSpace(firebaseSettings.ProjectId))
        {
            throw new InvalidOperationException("Firebase:ProjectId is not configured.");
        }

        var issuer = $"https://securetoken.google.com/{firebaseSettings.ProjectId}";

        services.AddScoped<CallerContext>();
        services.AddScoped<ICallerContext>(serviceProvider => serviceProvider.GetRequiredService<CallerContext>());
        services.AddScoped<FirebaseJwtBearerEvents>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = issuer;
                options.MapInboundClaims = false;
                options.EventsType = typeof(FirebaseJwtBearerEvents);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = firebaseSettings.ProjectId,
                    ValidateLifetime = true,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        // Authorization
        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            .AddPolicy(Policies.AdminOnly, policy => policy.RequireRole(nameof(UserRole.Admin)));

        // CORS
        var corsSettings = configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>() ?? new CorsSettings();

        services.AddCors(options => options.AddPolicy(CorsSettings.PolicyName, policy => policy
            .WithOrigins(corsSettings.AllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()));

        // Factories
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        // Repositories
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();

        // Services
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IVendorService, VendorService>();

        return services;
    }
}