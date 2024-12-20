 using Telegram.Bot.Types.Enums;
using UtilityBills.Telegram.Workflows.Core.Models;
using WorkflowCore.Interface;

namespace UtilityBills.Telegram.Extensions.WorkflowExtensions;

internal static class WorkflowHostExtensions
{
    private static readonly Dictionary<UpdateType, string> EventMap = new()
    {
        [UpdateType.Message] = "UserMessage",
        [UpdateType.CallbackQuery] = "CallbackMessage",
    };

    public static async Task PublishUserMessageAsync(this IWorkflowHost workflowHost, UpdateType updateType, string
        userId, UserMessage message)
    {
        await workflowHost.PublishEvent(EventMap[updateType], userId, message);
    }
}