using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;

namespace UtilityBills.Abstractions.Services;

public interface IDebtNotificationProvider
{
    Task NotifyAboutDebtAsync(string userId, decimal debt, UtilityPaymentPlatformType platform,
        CancellationToken ct = default);
}