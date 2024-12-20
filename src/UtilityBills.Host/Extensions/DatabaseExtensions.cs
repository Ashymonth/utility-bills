using Microsoft.EntityFrameworkCore;
using UtilityBills.Aggregates;
using UtilityBills.Infrastructure;

namespace UtilityBills.Host.Extensions;

internal static class DatabaseExtensions
{
    public static IServiceCollection AddConfiguredDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UtilityBillsDbContext>(builder =>
            builder.UseSqlite(configuration.GetConnectionString(nameof(UtilityBillsDbContext))));

        
        return services;
    }
}