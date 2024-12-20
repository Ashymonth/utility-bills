﻿using System.Linq.Expressions;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using UtilityBills.Telegram.Workflows.Core.Models;
using WorkflowCore.Interface;
using WorkflowCore.Primitives;

namespace UtilityBills.Telegram.Extensions.WorkflowExtensions;

internal static class WaitExtensions
{
    public static IStepBuilder<TData, WaitFor> WaitForUserInlineData<TData, TStep, TOutput>(
        this IStepBuilder<TData, TStep> builder,
        Expression<Func<TData, TOutput>> dataProperty)
        where TStep : IStepBody
        where TData : IUserStep
    {
        return builder.WaitForInternal("CallbackMessage", dataProperty);
    }

    public static IStepBuilder<TData, WaitFor> WaitForUserInlineData<TData, TStep, TOutput>(
        this IStepBuilder<TData, TStep> builder,
        Expression<Func<TData, TOutput>> dataProperty,
        Func<object, TOutput> dataConverter)
        where TStep : IStepBody
        where TData : IUserStep
    {
        return builder.WaitForInternal("CallbackMessage", dataProperty, dataConverter);
    }

    private static IStepBuilder<TData, WaitFor> WaitForInternal<TData, TStep, TOutput>(
        this IStepBuilder<TData, TStep> builder,
        string eventName,
        Expression<Func<TData, TOutput>> dataProperty,
        Func<object, TOutput>? dataConverter = null)
        where TStep : IStepBody
        where TData : IUserStep
    {
        return builder
            .WaitFor(eventName, data => data.UserId, _ => DateTime.UtcNow)
            .Output(dataProperty, step => dataConverter != null ? dataConverter(step.EventData) : step.EventData)
            .Output((@for, step) => step.SentMessageIds.Add((@for.EventData as UserMessage)!.MessageId));
    }
}