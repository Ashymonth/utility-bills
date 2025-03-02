using UtilityBills.Aggregates.ReadingPlatformAggregate.Services;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature.Steps;

public class EnsureMeterReadingsWereSentStep : IUserStep, IStepBody
{
    private readonly IMeterReadingsService _MeterReadingsService;

    public EnsureMeterReadingsWereSentStep(IMeterReadingsService MeterReadingsService)
    {
        _MeterReadingsService = MeterReadingsService;
    }

    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = [];

    public int HotWater { get; set; }

    public int ColdWater { get; set; }

    public bool IsMeterReadingsEquals { get; set; }

    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        var prevValue =
            await _MeterReadingsService.GetCurrentReadingsAsync(UserId, context.CancellationToken);

        IsMeterReadingsEquals = prevValue.Value.HotWater.Value == HotWater &&
                                     prevValue.Value.ColdWater!.Value == ColdWater;


        return ExecutionResult.Next();
    }
}