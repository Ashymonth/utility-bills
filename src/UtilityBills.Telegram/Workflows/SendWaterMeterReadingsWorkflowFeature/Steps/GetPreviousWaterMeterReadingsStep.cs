using FluentResults;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Models;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Services;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature.Steps;

public class GetPreviousWaterMeterReadingsStep : IUserStep, IStepBody
{
    private readonly IWaterMeterReadingsService _waterMeterReadingsService;

    public GetPreviousWaterMeterReadingsStep(IWaterMeterReadingsService waterMeterReadingsService)
    {
        _waterMeterReadingsService = waterMeterReadingsService;
    }

    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = [];

    public Result<WaterMeterReadingsPair> PreviousWaterMeterReadings { get; set; } = null!;

    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        PreviousWaterMeterReadings =
            await _waterMeterReadingsService.GetPreviousWaterMeterReadingsAsync(UserId, context.CancellationToken);

        return ExecutionResult.Next();
    }
}