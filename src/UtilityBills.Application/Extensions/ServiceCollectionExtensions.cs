using Microsoft.Extensions.DependencyInjection;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Services;
using UtilityBills.Application.Aggregates.UtilityPaymentPlatformAggregate.Services;
using UtilityBills.Application.Services;
using UtilityBills.Services;

namespace UtilityBills.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IUtilityPaymentPlatformService, UtilityPaymentPlatformService>();
        services.AddScoped<IWaterMeterReadingsService, WaterMeterReadingsService>();

        services.AddScoped<ICredentialsValidator, OrientCredentialsValidator>();
        services.AddScoped<ICredentialsValidator, KvadoCredentialsValidator>();

        services.AddScoped<IDebtNotificationManager, DebtNotificationManager>();
        services.AddScoped<IDebtNotificationService, DebtNotificationService>();

        return services;
    }
}