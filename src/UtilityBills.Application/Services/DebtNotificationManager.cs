using UtilityBills.Abstractions;
using UtilityBills.Entities;
using UtilityBills.Services;

namespace UtilityBills.Application.Services;

public class DebtNotificationManager : IDebtNotificationManager
{
    private readonly IRepository<User> _repository;
    private readonly IDebtNotificationService _notificationService;
    
    public DebtNotificationManager(IRepository<User> repository, IDebtNotificationService notificationService)
    {
        _repository = repository;
        _notificationService = notificationService;
    }

    public async Task StartJob(CancellationToken ct = default)
    {
        var users = await _repository.ListAsync(ct);

        foreach (var user in users)
        {
            await _notificationService.NotifyAboutDebtAsync(user.Id, ct);
        }
    }
}