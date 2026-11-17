using System.Security.Claims;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
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
        services.AddSingleton(_ => FirebaseApp.Create(new AppOptions
        {
            Credential = CreateFirebaseCredential(firebaseSettings),
            ProjectId = firebaseSettings.ProjectId,
        }));
        services.AddSingleton(serviceProvider => FirebaseAuth.GetAuth(serviceProvider.GetRequiredService<FirebaseApp>()));
        services.AddScoped<IIdentityService, FirebaseIdentityService>();

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
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();

        // Services
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ISalesOrderService, SalesOrderService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVendorService, VendorService>();
        services.AddScoped<IWorkOrderService, WorkOrderService>();

        return services;
    }

    private static GoogleCredential CreateFirebaseCredential(FirebaseSettings settings)
    {
        if (!string.IsNullOrWhiteSpace(settings.ServiceAccountJson))
        {
            return CredentialFactory.FromJson<ServiceAccountCredential>(settings.ServiceAccountJson).ToGoogleCredential();
        }

        if (!string.IsNullOrWhiteSpace(settings.ServiceAccountPath))
        {
            return CredentialFactory.FromFile<ServiceAccountCredential>(settings.ServiceAccountPath).ToGoogleCredential();
        }

        throw new InvalidOperationException("Firebase:ServiceAccountJson or Firebase:ServiceAccountPath is not configured.");
    }
}