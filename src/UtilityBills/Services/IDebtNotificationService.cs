namespace UtilityBills.Services;

public interface IDebtNotificationService
{
    Task NotifyAboutDebtAsync(string userId, CancellationToken ct = default);
}