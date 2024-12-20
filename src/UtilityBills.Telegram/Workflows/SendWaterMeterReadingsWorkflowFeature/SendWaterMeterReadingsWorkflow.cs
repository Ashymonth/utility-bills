using Microsoft.Extensions.Localization;
using UtilityBills.Telegram.Extensions.WorkflowExtensions;
using UtilityBills.Telegram.Workflows.Core.Steps;
using UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature.Extensions;
using UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature.Steps;
using WorkflowCore.Interface;

namespace UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature;

public class SendWaterMeterReadingsWorkflow : IWorkflow<SendWaterMeterReadingsWorkflowData>
{
    private readonly IStringLocalizer<SendWaterMeterReadingsWorkflow> _localizer;

    public SendWaterMeterReadingsWorkflow(IStringLocalizer<SendWaterMeterReadingsWorkflow> localizer)
    {
        _localizer = localizer;
    }

    public void Build(IWorkflowBuilder<SendWaterMeterReadingsWorkflowData> builder)
    {
        builder.Then<SendMessageToUser>()
            .Input(user => user.UserId, data => data.UserId)
            .Input(user => user.Message,
                data => _localizer.GetString("Input hot and cold water to send it in Orient and Kvado"))
            .Output(data => data.UserId, user => user.UserId)
            .RequestWaterMeterReadings(_localizer.GetString("Input hot water"), null)
            .WaitForUserMessage(data => data.HotWater, message => message.ToWaterMeterReadings(_localizer))
            .If(data => data.HotWater.IsFailed)
            .Do(workflowBuilder =>
                workflowBuilder
                    .SendMessageToUser(_localizer.GetString("Water meter readings must me a positive number"))
                    .EndWorkflow())
            .RequestWaterMeterReadings(_localizer.GetString("Input cold water"), _localizer.GetString("Skip"))
            .WaitForUserMessage(data => data.ColdWater, message => message.ToWaterMeterReadings(_localizer))
            .If(data => data.ColdWater.IsFailed)
            .Do(workflowBuilder =>
                workflowBuilder
                    .SendMessageToUser(_localizer.GetString("Water meter readings must me a positive number"))
                    .EndWorkflow())
            .If(data => data.HotWater.ValueOrDefault == null && data.ColdWater.ValueOrDefault == null)
            .Do(workflowBuilder =>
                workflowBuilder.SendMessageToUser(_localizer.GetString(
                        "To send water meter readings you need to input hold or cold water meter readings"))
                    .EndWorkflow())
            .Then<SendWaterMeterReadingsStep>()
            .Input(step => step.UserId, data => data.UserId)
            .Input(step => step.HotWater, data => data.HotWater.Value)
            .Input(step => step.ColdWater, data => data.ColdWater.ValueOrDefault)
            .Output(data => data.Result, step => step.Result)
            .SendMessageToUser(data => _localizer.GetString(data.Result.IsSuccess ? "Water meter readings sent" : data.Result.Errors.First().Message))
            .EndWorkflow();
    }

    public string Id => nameof(SendWaterMeterReadingsWorkflow);

    public int Version => 1;
}