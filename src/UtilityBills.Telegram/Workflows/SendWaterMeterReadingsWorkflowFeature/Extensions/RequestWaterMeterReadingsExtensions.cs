using FluentResults;
using Microsoft.Extensions.Localization;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using UtilityBills.Telegram.Workflows.Core.Models;
using UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature.Steps;
using WorkflowCore.Interface;

namespace UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature.Extensions;

internal static class RequestWaterMeterReadingsExtensions
{
    public static IStepBuilder<TData, RequestWaterMeterReadings> RequestWaterMeterReadings<TData, TStep>(
        this IStepBuilder<TData, TStep> builder, string meterReadingsName, string? skipButtonName)
        where TData : IUserStep where TStep : IStepBody
    {
        return builder
            .Then<RequestWaterMeterReadings>()
            .Input(readings => readings.UserId, data => data.UserId)
            .Input(readings => readings.WaterMeterReadingsName, data => meterReadingsName)
            .Input(readings => readings.SkipButtonName, data => skipButtonName)
            .Output(step => step.UserId, readings => readings.UserId);
    }

    public static Result<WaterMeterReadings?> ToWaterMeterReadings<TLocalizer>(this UserMessage message,
        IStringLocalizer<TLocalizer> localizer)
    {
        if (message.Message == "Skip")
        {
            return Result.Ok();
        }

        if (!int.TryParse(message.Message, out var number))
        {
            return Result.Fail(
                localizer.GetString("Water meter readings must be number and greatest that 0"));
        }

        return WaterMeterReadings.Create(number)!;
    }
}