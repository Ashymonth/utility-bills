using System.Linq.Expressions;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using UtilityBills.Telegram.Workflows.Core.Models;
using UtilityBills.Telegram.Workflows.Core.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Primitives;

namespace UtilityBills.Telegram.Extensions.WorkflowExtensions;

public static class StepBuilderMessageExtensions
{
    public static IStepBuilder<TData, SendMessageToUser> SendMessageToUser<TData>(
        this IWorkflowBuilder<TData> builder, string message)
        where TData : IUserStep
    {
        return builder
            .StartWith<SendMessageToUser>()
            .Input(step => step.UserId, data => data.UserId)
            .Input(step => step.Message, _ => message)
            .Input(user => user.SentMessageIds, step => step.SentMessageIds)
            .Output(step => step.SentMessageIds, user => user.SentMessageIds);
    }

    public static IStepBuilder<TData, SendMessageToUser> SendMessageToUser<TData, TStep>(
        this IStepBuilder<TData, TStep> builder, string message)
        where TData : IUserStep where TStep : IStepBody
    {
        return builder
            .Then<SendMessageToUser>()
            .Input(step => step.UserId, data => data.UserId)
            .Input(step => step.Message, _ => message)
            .Input(user => user.SentMessageIds, step => step.SentMessageIds)
            .Output(step => step.SentMessageIds, user => user.SentMessageIds);
    }


    public static IStepBuilder<TData, SendMessageToUser> SendMessageToUser<TData>(
        this IWorkflowBuilder<TData> builder, Func<TData, string> messageFunc)
        where TData : IUserStep
    {
        return builder
            .StartWith<SendMessageToUser>()
            .Input(step => step.UserId, data => data.UserId)
            .Input(step => step.Message, data => messageFunc(data))
            .Input(user => user.SentMessageIds, step => step.SentMessageIds)
            .Output(step => step.SentMessageIds, user => user.SentMessageIds);
    }

    public static IStepBuilder<TData, SendMessageToUser> SendMessageToUser<TData, TStep>(
        this IStepBuilder<TData, TStep> builder, Func<TData, string> messageFunc)
        where TStep : IStepBody
        where TData : IUserStep
    {
        return builder
            .Then<SendMessageToUser>()
            .Input(step => step.UserId, data => data.UserId)
            .Input(step => step.Message, data => messageFunc(data))
            .Input(user => user.SentMessageIds, step => step.SentMessageIds)
            .Output(step => step.SentMessageIds, user => user.SentMessageIds);
    }

    public static IStepBuilder<TData, WaitFor> WaitForUserMessage<TData, TStep, TOutput>(
        this IStepBuilder<TData, TStep> builder,
        Expression<Func<TData, TOutput>> dataProperty)
        where TStep : IStepBody
        where TData : IUserStep
    {
        return builder
            .WaitFor("UserMessage", data => data.UserId, _ => DateTime.UtcNow)
            .Output(dataProperty, step => step.EventData);
    }

    public static IStepBuilder<TData, WaitFor> WaitForUserMessage<TData, TStep, TOutput>(
        this IStepBuilder<TData, TStep> builder,
        Expression<Func<TData, TOutput>> dataProperty, Func<UserMessage, TOutput> dataConverter)
        where TStep : IStepBody
        where TData : IUserStep
    {
        return builder
            .WaitFor("UserMessage", (step, context) => step.UserId, _ => DateTime.UtcNow)
            .Output(dataProperty, step => dataConverter((step.EventData as UserMessage)!));
    }
}