using FluentResults;
using Microsoft.Extensions.Localization;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using UtilityBills.Telegram.Workflows.Core.Models;
using UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature.Steps;
using WorkflowCore.Interface;

namespace UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature.Extensions;

internal static class RequestMeterReadingsExtensions
{
    public static IStepBuilder<TData, RequestMeterReadings> RequestMeterReadings<TData, TStep>(
        this IStepBuilder<TData, TStep> builder, string meterReadingsName)
        where TData : IUserStep where TStep : IStepBody
    {
        return builder
            .Then<RequestMeterReadings>()
            .Input(readings => readings.UserId, data => data.UserId)
            .Input(readings => readings.MeterReadingsName, data => meterReadingsName)
            .Output(step => step.UserId, readings => readings.UserId);
    }

    public static Result<MeterReadings> ToMeterReadings<TLocalizer>(this UserMessage message,
        IStringLocalizer<TLocalizer> localizer)
    {
        if (!int.TryParse(message.Message, out var number))
        {
            return Result.Fail(
                localizer.GetString("Water meter readings must be number and greatest that 0"));
        }

        return MeterReadings.Create(number)!;
    }
}