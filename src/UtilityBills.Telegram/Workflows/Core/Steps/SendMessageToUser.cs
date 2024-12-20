using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.Core.Steps;

public class SendMessageToUser : IStepBody, IUserStep
{
    private readonly ITelegramBotClient _telegramBotClient;

    public SendMessageToUser(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public string UserId { get; set; } = default!;

    public List<int> SentMessageIds { get; set; } = [];

    public string Message { get; set; } = null!;

    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        _ = await _telegramBotClient.SendMessage(UserId, Message, replyMarkup: new ReplyKeyboardRemove());

        return ExecutionResult.Next();
    }
}