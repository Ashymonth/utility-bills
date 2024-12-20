using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

namespace UtilityBills.Abstractions.Services;

public interface IDebtProvider
{
    Task<decimal?> GetDebtAsync(Email email, Password password, CancellationToken ct = default);
}