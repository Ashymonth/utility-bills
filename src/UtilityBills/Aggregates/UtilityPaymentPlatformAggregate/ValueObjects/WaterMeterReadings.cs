using FluentResults;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

public record MeterReadings
{
    // For ef
    // ReSharper disable once UnusedMember.Local
    private MeterReadings()
    {
    }

    private MeterReadings(int value)
    {
        Value = value;
    }

    public int Value { get; private set; }

    public static Result<MeterReadings> Create(int value)
    {
        return value <= 0
            ? Result.Fail("Water meter readings can't be less that 0")
            : Result.Ok(new MeterReadings(value));
    }
}