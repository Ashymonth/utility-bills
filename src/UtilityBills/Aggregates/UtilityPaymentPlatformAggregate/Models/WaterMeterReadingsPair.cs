using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Models;

public class WaterMeterReadingsPair
{
    private WaterMeterReadingsPair()
    {
    }

    public WaterMeterReadings HotWater { get; private set; } = null!;

    public WaterMeterReadings? ColdWater { get; private set; }

    public static WaterMeterReadingsPair Create(WaterMeterReadings hotWater, WaterMeterReadings? coldWater)
    {
        return new WaterMeterReadingsPair
        {
            HotWater = hotWater,
            ColdWater = coldWater
        };
    }
}