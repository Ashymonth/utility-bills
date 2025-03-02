using FluentResults;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Models;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;
using UtilityBills.Telegram.Workflows.Core.Abstractions;

namespace UtilityBills.Telegram.Workflows.SendMeterReadingsWorkflowFeature;

public class SendMeterReadingsWorkflowData : IUserStep
{
    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = null!;

    public Result<MeterReadings> HotWater { get; set; } = null!;

    public Result<MeterReadingsPair> PreviousMeterReadings { get; set; } = null!;

    public Result<MeterReadings> ColdWater { get; set; } = null!;

    public Result Result { get; set; } = null!;

    public bool IsSentMeterReadingsAccepted { get; set; }

    public bool IsHotWaterValid(Result<MeterReadings> meterReadings)
    {
        return meterReadings.IsSuccess && PreviousMeterReadings.Value.HotWater.Value < meterReadings.Value.Value;
    }

    public int GetPrevHotWater()
    {
        return PreviousMeterReadings.Value.HotWater.Value;
    }

    public int GetPrevColdWater()
    {
        return PreviousMeterReadings.Value.ColdWater?.Value ?? 0;
    }
}