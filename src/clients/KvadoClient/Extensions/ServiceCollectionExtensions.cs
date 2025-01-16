using KvadoClient.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace KvadoClient.Extensions;

public static class ServiceCollectionExtensions
{
    private const string BaseAddress = "https://cabinet.kvado.ru";

    /// <summary>
    /// Add http client to connect to https://cabinet.kvado.ru/
    /// </summary>
    /// <param name="services">Services</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddKvadoClient(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMemoryCache();

        services.AddTransient<ICookieProvider, CookieProvider>();
        services.AddTransient<CookieDelegateHandler>();

        services.AddHttpClient<IKvadoAuthorizationClient, KvadoAuthorizationClient>(client =>
            {
                client.Timeout = TimeSpan.FromHours(1);
                client.BaseAddress = new Uri(BaseAddress);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });

        services.AddHttpClient<IKvadoHttpClient, KvadoHttpClient>(client =>
            {
                client.Timeout = TimeSpan.FromHours(1);
                client.BaseAddress = new Uri(BaseAddress);
            })
            .AddHttpMessageHandler<CookieDelegateHandler>();

        return services;
    }
}