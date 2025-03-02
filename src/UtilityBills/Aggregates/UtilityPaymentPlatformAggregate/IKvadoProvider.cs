using FluentResults;
using UtilityBills.Abstractions.Services;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Models;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;

public interface IKvadoProvider : IDebtProvider
{
    Task<Result> AuthenticateAsync(Email email, Password password, CancellationToken ct = default);

    Task<Result> SendMeterReadingsAsync(Email email, Password password, MeterReadings hotWater,
        MeterReadings coldWater, CancellationToken ct = default);

    Task<Result<MeterReadingsPair>> GetPreviousMeterReadingsAsync(
        Email email, Password password, CancellationToken ct = default);
    
    Task<Result<MeterReadingsPair>> GetCurrentMeterReadingsAsync(Email email, Password password,
        CancellationToken ct = default);
}