using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Repositories;
using SavageMetals.Api.DAL;
using SavageMetals.Api.Factories;
using SavageMetals.Api.Middleware;
using SavageMetals.Api.Services;

namespace SavageMetals.Api;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSavageMetalsServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Error handling
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // Factories
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        // Repositories
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        // Services
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}