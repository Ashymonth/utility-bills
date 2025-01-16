using Microsoft.Extensions.DependencyInjection;
using OrientClient.Clients;

namespace OrientClient.Extensions;

/// <summary>
/// Extensions to add orient client in DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string BaseAddress = "https://lk.hppp.ru";
    
    /// <summary>
    /// Add http client to connect to https://lk.hppp.ru
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddOrientClient(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddHttpClient<IOrientClient, OrientHttpClient>(client => client.BaseAddress = new Uri(BaseAddress))
            .AddHttpMessageHandler<CookieDelegateHandler>();

        services.AddHttpClient<IOrientAuthorizationClient, OrientAuthorizationClient>(client =>
            client.BaseAddress = new Uri(BaseAddress));

        services.AddTransient<IUserProvider, UserProvider>();
        services.AddTransient<CookieDelegateHandler>();

        services.AddMemoryCache();

        return services;
    }
}