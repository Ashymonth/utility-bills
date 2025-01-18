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
        builder
            .StartWith<GetPreviousWaterMeterReadingsStep>()
            .Input(step => step.UserId, data => data.UserId)
            .Output(data => data.PreviousWaterMeterReadings, step => step.PreviousWaterMeterReadings)
            .If(data => data.PreviousWaterMeterReadings.IsFailed)
            .Do(workflowBuilder => workflowBuilder.SendMessageToUser("Unable to get previous water meter readings.")
                .EndWorkflow())
            .Then<SendMessageToUser>()
            .Input(user => user.UserId, data => data.UserId)
            .Input(user => user.Message,
                data => _localizer.GetString("Input hot and cold water to send it in Orient and Kvado\n" +
                                             "Previous max value:\n" +
                                             $"Hot water: {data.GetPrevHotWater()}\n" +
                                             $"Cold water: {data.GetPrevColdWater()}"))
            .Output(data => data.UserId, user => user.UserId)
            .RequestWaterMeterReadings(_localizer.GetString("Input hot water"))
            .WaitForUserMessage(data => data.HotWater, message => message.ToWaterMeterReadings(_localizer))
            .If(data => data.HotWater.IsFailed)
            .Do(workflowBuilder =>
                workflowBuilder
                    .SendMessageToUser(_localizer.GetString("Water meter readings must me a positive number"))
                    .EndWorkflow())
            .If(data => data.HotWater.Value!.Value < data.GetPrevHotWater())
            .Do(workflowBuilder => workflowBuilder.SendMessageToUser(data =>
                $"Hot water can't be less that previous value. Previous value: {data.GetPrevHotWater()}."))
            .RequestWaterMeterReadings(_localizer.GetString("Input cold water"))
            .WaitForUserMessage(data => data.ColdWater, message => message.ToWaterMeterReadings(_localizer))
            .If(data => data.ColdWater.IsFailed)
            .Do(workflowBuilder =>
                workflowBuilder
                    .SendMessageToUser(_localizer.GetString("Water meter readings must me a positive number"))
                    .EndWorkflow())
            .If(data => data.ColdWater.Value!.Value < data.GetPrevColdWater() )
            .Do(workflowBuilder => workflowBuilder.SendMessageToUser(data =>
                $"Cold water can't be less that previous value. Previous value: {data.GetPrevColdWater()}."))
            .RequestWaterMeterReadings(_localizer.GetString("Input cold water"))
            .Then<SendWaterMeterReadingsStep>()
            .Input(step => step.UserId, data => data.UserId)
            .Input(step => step.HotWater, data => data.HotWater.Value)
            .Input(step => step.ColdWater, data => data.ColdWater.ValueOrDefault)
            .Output(data => data.Result, step => step.Result)
            .SendMessageToUser(data =>
                _localizer.GetString(data.Result.IsSuccess
                    ? "Water meter readings sent"
                    : data.Result.Errors.First().Message))
            .EndWorkflow();
    }

    public string Id => nameof(SendWaterMeterReadingsWorkflow);

    public int Version => 1;
}