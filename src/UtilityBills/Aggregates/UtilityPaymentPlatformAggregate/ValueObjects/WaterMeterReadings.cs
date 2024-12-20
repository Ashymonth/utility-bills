using FluentResults;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

public record WaterMeterReadings
{
    // For ef
    // ReSharper disable once UnusedMember.Local
    private WaterMeterReadings()
    {
    }

    private WaterMeterReadings(int value)
    {
        Value = value;
    }

    public int Value { get; private set; }

    public static Result<WaterMeterReadings> Create(int value)
    {
        return value <= 0
            ? Result.Fail("Water meter readings can't be less that 0")
            : Result.Ok(new WaterMeterReadings(value));
    }
}