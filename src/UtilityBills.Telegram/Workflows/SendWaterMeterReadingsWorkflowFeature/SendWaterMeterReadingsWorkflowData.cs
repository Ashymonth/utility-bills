using FluentResults;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Models;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;
using UtilityBills.Telegram.Workflows.Core.Abstractions;

namespace UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature;

public class SendWaterMeterReadingsWorkflowData : IUserStep
{
    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = null!;

    public Result<WaterMeterReadings> HotWater { get; set; } = null!;

    public Result<WaterMeterReadingsPair> PreviousWaterMeterReadings { get; set; } = null!;
    
    public Result<WaterMeterReadings> ColdWater { get; set; } = null!;

    public Result Result { get; set; } = null!;

    public int GetPrevHotWater()
    {
        return PreviousWaterMeterReadings.Value.HotWater.Value;
    }
    
    public int GetPrevColdWater()
    {
        return PreviousWaterMeterReadings.Value.ColdWater?.Value ?? 0;
    }
}