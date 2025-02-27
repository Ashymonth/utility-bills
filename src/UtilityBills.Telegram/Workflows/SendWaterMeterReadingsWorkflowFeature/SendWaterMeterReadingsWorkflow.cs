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
            .While(data => !data.IsHotWaterValid(data.HotWater))
            .Do(workflowBuilder =>
                workflowBuilder
                    .SendMessageToUser(_localizer.GetString("Water meter readings must me a positive number or greatest that previous value")))
            .While(data => data.HotWater.Value!.Value < data.GetPrevHotWater())
            .Do(workflowBuilder => workflowBuilder.SendMessageToUser(data =>
                $"Hot water can't be less that previous value. Previous value: {data.GetPrevHotWater()}."))
            .SendMessageToUser(_localizer.GetString("Input cold water"))
            .WaitForUserMessage(data => data.ColdWater, message => message.ToWaterMeterReadings(_localizer))
            .While(data => data.ColdWater.IsFailed)
            .Do(workflowBuilder =>
                workflowBuilder
                    .SendMessageToUser(_localizer.GetString("Water meter readings must me a positive number")))
            .While(data => data.ColdWater.Value!.Value < data.GetPrevColdWater())
            .Do(workflowBuilder => workflowBuilder.SendMessageToUser(data =>
                $"Cold water can't be less that previous value. Previous value: {data.GetPrevColdWater()}."))
            .Then<SendWaterMeterReadingsStep>()
            .Input(step => step.UserId, data => data.UserId)
            .Input(step => step.HotWater, data => data.HotWater.Value)
            .Input(step => step.ColdWater, data => data.ColdWater.ValueOrDefault)
            .Output(data => data.Result, step => step.Result)
            .Then<EnsureWaterMeterReadingsWereSentStep>()
            .Input(step => step.UserId, data => data.UserId)
            .Input(step => step.HotWater, data => data.HotWater.Value.Value)
            .Input(step => step.ColdWater, data => data.ColdWater.Value.Value)
            .Output(data => data.IsSentWaterMeterReadingsAccepted, step => step.IsWaterMeterReadingsEquals)
            .If(data => data.IsSentWaterMeterReadingsAccepted).Do(workflowBuilder => workflowBuilder.SendMessageToUser(
                data => $"Water meter readings successfully sent:\n" +
                        $"How water: {data.HotWater.Value.Value}.\n" +
                        $"Cold water: {data.ColdWater.Value.Value}.\n")
                .EndWorkflow())
            .SendMessageToUser("An error occured while sending water meter readings. Please retry later")
            .EndWorkflow();
    }

    public string Id => nameof(SendWaterMeterReadingsWorkflow);

    public int Version => 1;
}