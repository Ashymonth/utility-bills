using Telegram.Bot;
using UtilityBills.Telegram.Workflows.Core.Abstractions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature.Steps;

public class RequestWaterMeterReadings : IStepBody, IUserStep
{
    private readonly ITelegramBotClient _telegramBotClient;

    public RequestWaterMeterReadings(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = [];

    public string WaterMeterReadingsName { get; set; } = null!;

    public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(UserId);
        ArgumentException.ThrowIfNullOrWhiteSpace(WaterMeterReadingsName);

        await _telegramBotClient.SendMessage(int.Parse(UserId), WaterMeterReadingsName);

        return ExecutionResult.Next();
    }
}