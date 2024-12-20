using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.Core.Steps;

public class SendInlineDataMessageToUser : IUserStep, IStepBody
{
    private readonly ITelegramBotClient _telegramBotClient;

    public SendInlineDataMessageToUser(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = null!;

    public string Message { get; set; } = null!;

    public InlineKeyboardButton[] Options { get; set; } = [];

    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        var replyMarkup = new InlineKeyboardMarkup(Options);

        var result =
            await _telegramBotClient.SendMessage(int.Parse(UserId), Message, replyMarkup: replyMarkup);

        SentMessageIds.Add(result.MessageId);

        return ExecutionResult.Next();
    }
}