using UtilityBills.Aggregates.ReadingPlatformAggregate;

namespace UtilityBills.Abstractions.Services;

public interface IDebtNotificationProvider
{
    Task NotifyAboutDebtAsync(string userId, decimal debt, ReadingPlatformType platform,
        CancellationToken ct = default);
}