using Microsoft.Extensions.Logging;
using UtilityBills.Abstractions;
using UtilityBills.Abstractions.Services;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Specifications;
using UtilityBills.Services;

namespace UtilityBills.Application.Services;

public class DebtNotificationService : IDebtNotificationService
{
    private readonly Dictionary<ReadingPlatformType, IDebtProvider> _debtProviders;
    private readonly IRepository<ReadingPlatform> _repository;
    private readonly IDebtNotificationProvider _notificationProvider;
    private readonly ILogger<DebtNotificationService>? _logger;

    public DebtNotificationService(IOrientProvider orientProvider, IKvadoProvider kvadoProvider,
        IRepository<ReadingPlatform> repository, IDebtNotificationProvider notificationProvider,
        ILogger<DebtNotificationService>? logger = null)
    {
        _debtProviders = new Dictionary<ReadingPlatformType, IDebtProvider>
        {
            [ReadingPlatformType.Orient] = orientProvider,
            [ReadingPlatformType.Kvado] = kvadoProvider
        };
        _repository = repository;
        _notificationProvider = notificationProvider;
        _logger = logger;
    }

    public async Task NotifyAboutDebtAsync(string userId, CancellationToken ct = default)
    {
        var platforms = await _repository.ListAsync(new GetAllPlatformsWithCredentials(), ct);

        await Parallel.ForEachAsync(platforms, ct, async (platform, token) =>
        {
            var credentials = platform.GetUserCredential(userId);

            if (credentials is null)
            {
                _logger.LogInformation("User: {UserId} don't have credentials for platform: {Platform}", userId,
                    platform.PlatformType);
                return;
            }

            var debtProvider = _debtProviders[platform.PlatformType];

            var debt = await debtProvider.GetDebtAsync(credentials.Email, credentials.Password, token);
            if (debt.HasValue)
            {
                await _notificationProvider.NotifyAboutDebtAsync(userId, debt.Value, platform.PlatformType, token);
            }
        });
    }
}