using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate.Models;

public class MeterReadingsPair
{
    private MeterReadingsPair()
    {
    }

    public MeterReadings HotWater { get; private set; } = null!;

    public MeterReadings ColdWater { get; private set; }

    public static MeterReadingsPair? Create(MeterReadings hotWater, MeterReadings coldWater)
    {
        return new MeterReadingsPair
        {
            HotWater = hotWater,
            ColdWater = coldWater
        };
    }
}