using Microsoft.EntityFrameworkCore;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Infrastructure;

namespace UtilityBills.Host.Extensions;

internal static class DatabaseExtensions
{
    public static IServiceCollection AddConfiguredDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UtilityBillsDbContext>(builder =>
        {
            builder.UseAsyncSeeding(async (context, b, token) =>
            {
                if (await context.Set<ReadingPlatform>().AnyAsync(token))
                {
                    return;
                }

                context.Set<ReadingPlatform>().AddRange(
                    ReadingPlatform.Create("Ориетн", ReadingPlatformType.Orient),
                    ReadingPlatform.Create("Квадо", ReadingPlatformType.Kvado),
                    ReadingPlatform.Create("Рус энерго", ReadingPlatformType.RusEnergy));
                
                await context.SaveChangesAsync(token);
            });

            builder.UseNpgsql(configuration.GetConnectionString(nameof(UtilityBillsDbContext)));
        });


        return services;
    }
}