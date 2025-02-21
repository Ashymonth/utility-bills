using FluentResults;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Services;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature.Steps;

public class EnsureWaterMeterReadingsWereSentStep : IUserStep, IStepBody
{
    private readonly IWaterMeterReadingsService _waterMeterReadingsService;

    public EnsureWaterMeterReadingsWereSentStep(IWaterMeterReadingsService waterMeterReadingsService)
    {
        _waterMeterReadingsService = waterMeterReadingsService;
    }

    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = [];

    public int HotWater { get; set; }

    public int ColdWater { get; set; }

    public bool IsWaterMeterReadingsEquals { get; set; }

    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        var prevValue =
            await _waterMeterReadingsService.GetCurrentWaterMeterReadingsAsync(UserId, context.CancellationToken);

        IsWaterMeterReadingsEquals = prevValue.Value.HotWater.Value == HotWater &&
                                     prevValue.Value.ColdWater!.Value == ColdWater;


        return ExecutionResult.Next();
    }
}