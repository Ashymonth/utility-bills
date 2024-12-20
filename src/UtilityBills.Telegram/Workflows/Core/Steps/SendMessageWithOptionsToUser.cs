using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.Core.Steps;

public class SendMessageWithOptionsToUser : IStepBody, IUserStep
{
    private readonly ITelegramBotClient _telegramBotClient;

    public SendMessageWithOptionsToUser(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public string UserId { get; set; } = default!;

    public List<int> SentMessageIds { get; set; } = null!;

    public string Message { get; set; } = default!;

    public string[] Options { get; set; } = null!;

    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        var answerOptions = new ReplyKeyboardMarkup(new[]
        {
            Options.Select(x => new KeyboardButton(x))
        })
        {
            ResizeKeyboard = true
        };

        var message =await _telegramBotClient.SendTextMessageAsync(UserId, Message, replyMarkup: answerOptions);
        SentMessageIds.Add(message.MessageId);
        
        return ExecutionResult.Next();
    }
}