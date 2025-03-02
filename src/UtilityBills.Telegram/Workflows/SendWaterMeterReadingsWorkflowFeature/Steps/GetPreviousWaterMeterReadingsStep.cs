using FluentResults;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Models;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Services;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature.Steps;

public class GetPreviousMeterReadingsStep : IUserStep, IStepBody
{
    private readonly IMeterReadingsService _MeterReadingsService;

    public GetPreviousMeterReadingsStep(IMeterReadingsService MeterReadingsService)
    {
        _MeterReadingsService = MeterReadingsService;
    }

    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = [];

    public Result<MeterReadingsPair> PreviousMeterReadings { get; set; } = null!;

    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        PreviousMeterReadings =
            await _MeterReadingsService.GetPreviousReadingsAsync(UserId, context.CancellationToken);

        return ExecutionResult.Next();
    }
}