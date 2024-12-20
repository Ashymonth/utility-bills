using System.Linq.Expressions;
using Telegram.Bot.Types.ReplyMarkups;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using UtilityBills.Telegram.Workflows.Core.Steps;
using WorkflowCore.Interface;

namespace UtilityBills.Telegram.Extensions.WorkflowExtensions;

public static class StepBuilderInlineDataExtensions
{
    public static IStepBuilder<TData, SendMessageWithOptionsToUser> SendVariants<TData, TStep>(
        this IStepBuilder<TData, TStep> builder, string message, string[] variants)
        where TStep : IStepBody
        where TData : IUserStep
    {
        var step = builder.Then<SendMessageWithOptionsToUser>();
        return SendVariantsInternal(step, message, _ => variants);
    }

    public static IStepBuilder<TData, SendMessageWithOptionsToUser> SendVariants<TData, TStep>(
        this IStepBuilder<TData, TStep> builder, string message, Expression<Func<TData, string[]>> getVariants)
        where TStep : IStepBody
        where TData : IUserStep
    {
        var step = builder.Then<SendMessageWithOptionsToUser>();
        return SendVariantsInternal(step, message, getVariants);
    }

    public static IStepBuilder<TData, SendInlineDataMessageToUser> SendInlineData<TData, TStep>(
        this IStepBuilder<TData, TStep> builder, string message,
        Expression<Func<TData, InlineKeyboardButton[]>> getVariants)
        where TStep : IStepBody
        where TData : IUserStep
    {
        return builder
            .Then<SendInlineDataMessageToUser>()
            .Input(step => step.UserId, data => data.UserId)
            .Input(step => step.Message, _ => message)
            .Input(step => step.Options, getVariants)
            .Input(user => user.SentMessageIds, step => step.SentMessageIds)
            .Output(step => step.SentMessageIds, user => user.SentMessageIds);
    }

    private static IStepBuilder<TData, SendMessageWithOptionsToUser> SendVariantsInternal<TData>(
        this IStepBuilder<TData, SendMessageWithOptionsToUser> builder, string message,
        Expression<Func<TData, string[]>> getVariants)
        where TData : IUserStep
    {
        return builder
            .Input(step => step.UserId, data => data.UserId)
            .Input(step => step.Message, _ => message)
            .Input(step => step.Options, getVariants)
            .Input(user => user.SentMessageIds, step => step.SentMessageIds)
            .Output(step => step.SentMessageIds, user => user.SentMessageIds);
    }
}