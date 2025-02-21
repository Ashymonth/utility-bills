using FluentResults;
using UtilityBills.Abstractions.Services;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Models;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;

public interface IKvadoProvider : IDebtProvider
{
    Task<Result> AuthenticateAsync(Email email, Password password, CancellationToken ct = default);

    Task<Result> SendWaterMeterReadingsAsync(Email email, Password password, WaterMeterReadings hotWater,
        WaterMeterReadings coldWater, CancellationToken ct = default);

    Task<Result<WaterMeterReadingsPair>> GetPreviousWaterMeterReadingsAsync(
        Email email, Password password, CancellationToken ct = default);
    
    Task<Result<WaterMeterReadingsPair>> GetCurrentWaterMeterReadingsAsync(Email email, Password password,
        CancellationToken ct = default);
}