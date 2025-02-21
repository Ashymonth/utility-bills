using FluentResults;
using UtilityBills.Abstractions.Services;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Models;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;

public interface IOrientProvider : IDebtProvider
{
    Task<Result> AuthenticateAsync(Email email, Password password, CancellationToken ct = default);

    Task<Result> SendWaterMeterReadingsAsync(Email email, Password password, WaterMeterReadings hotWater,
        CancellationToken ct = default);

    Task<Result<DateOnly>> GetLastDayWhenWaterMeterReadingsWereSent(Email email, Password password,
        CancellationToken ct = default);
    
    Task<Result<WaterMeterReadingsPair>> GetPreviousWaterMeterReadingAsync(Email email, Password password,
        CancellationToken ct = default);
}