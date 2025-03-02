using Microsoft.Extensions.DependencyInjection;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Services;
using UtilityBills.Application.Aggregates.ReadingPlatformAggregate.Services;
using UtilityBills.Application.Services;
using UtilityBills.Services;

namespace UtilityBills.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IReadingPlatformService, ReadingPlatformService>();
        services.AddScoped<IMeterReadingsService, MeterReadingsService>();

        services.AddScoped<ICredentialsValidator, OrientCredentialsValidator>();
        services.AddScoped<ICredentialsValidator, KvadoCredentialsValidator>();

        services.AddScoped<IDebtNotificationManager, DebtNotificationManager>();
        services.AddScoped<IDebtNotificationService, DebtNotificationService>();

        return services;
    }
}