using FluentResults;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Services;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature.Steps;

public class SendMeterReadingsStep : IStepBody, IUserStep
{
    private readonly IMeterReadingsService _MeterReadingsService;

    public SendMeterReadingsStep(IMeterReadingsService MeterReadingsService)
    {
        _MeterReadingsService = MeterReadingsService;
    }

    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = null!;

    public MeterReadings HotWater { get; set; } = null!;

    public MeterReadings PreviousHotWater { get; set; }
    
    public MeterReadings? ColdWater { get; set; }

    public MeterReadings PreviousColdWater { get; set; }

    public Result Result { get; set; } = null!;

    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        Result = await _MeterReadingsService.SendReadingsAsync(UserId, HotWater, ColdWater, context.CancellationToken);

        return ExecutionResult.Next();
    }
}