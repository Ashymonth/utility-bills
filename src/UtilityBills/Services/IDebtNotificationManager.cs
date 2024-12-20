namespace UtilityBills.Services;

public interface IDebtNotificationManager
{
    Task StartJob(CancellationToken ct = default);
}