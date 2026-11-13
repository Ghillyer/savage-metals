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
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}