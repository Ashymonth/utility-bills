using Microsoft.Extensions.DependencyInjection;
using UtilityBills.Abstractions;
using UtilityBills.Infrastructure.Repositories;

namespace UtilityBills.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        
        return services;
    }
}