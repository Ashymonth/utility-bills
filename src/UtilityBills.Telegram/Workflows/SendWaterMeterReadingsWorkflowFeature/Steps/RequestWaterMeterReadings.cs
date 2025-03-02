using Telegram.Bot;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.SendMeterReadingsWorkflowFeature.Steps;

public class RequestMeterReadings : IStepBody, IUserStep
{
    private readonly ITelegramBotClient _telegramBotClient;

    public RequestMeterReadings(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = [];

    public string MeterReadingsName { get; set; } = null!;

    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(UserId);
        ArgumentException.ThrowIfNullOrWhiteSpace(MeterReadingsName);

        await _telegramBotClient.SendMessage(int.Parse(UserId), MeterReadingsName);

        return ExecutionResult.Next();
    }
}