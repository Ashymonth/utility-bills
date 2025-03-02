using FluentResults;
using UtilityBills.Abstractions.Services;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Models;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;

public interface IOrientProvider : IDebtProvider
{
    Task<Result> AuthenticateAsync(Email email, Password password, CancellationToken ct = default);

    Task<Result> SendMeterReadingsAsync(Email email, Password password, MeterReadings hotWater,
        CancellationToken ct = default);

    Task<Result<DateOnly>> GetLastDayWhenMeterReadingsWereSent(Email email, Password password,
        CancellationToken ct = default);
    
    Task<Result<MeterReadingsPair>> GetPreviousWaterMeterReadingAsync(Email email, Password password,
        CancellationToken ct = default);
}