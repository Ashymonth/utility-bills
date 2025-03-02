using FluentResults;
using UtilityBills.Abstractions.Services;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Models;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate;

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