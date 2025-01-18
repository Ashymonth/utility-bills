using FluentResults;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Services;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature.Steps;

public class SendWaterMeterReadingsStep : IStepBody, IUserStep
{
    private readonly IWaterMeterReadingsService _waterMeterReadingsService;

    public SendWaterMeterReadingsStep(IWaterMeterReadingsService waterMeterReadingsService)
    {
        _waterMeterReadingsService = waterMeterReadingsService;
    }

    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = null!;

    public WaterMeterReadings HotWater { get; set; } = null!;

    public WaterMeterReadings PreviousHotWater { get; set; }
    
    public WaterMeterReadings? ColdWater { get; set; }

    public WaterMeterReadings PreviousColdWater { get; set; }

    public Result Result { get; set; } = null!;

    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        Result = await _waterMeterReadingsService.SendAsync(UserId, HotWater, ColdWater, context.CancellationToken);

        return ExecutionResult.Next();
    }
}