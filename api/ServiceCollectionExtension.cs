using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Factories;

namespace SavageMetals.Api;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSavageMetalsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        return services;
    }
}