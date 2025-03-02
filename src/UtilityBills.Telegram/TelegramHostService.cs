using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;
using UtilityBills.Telegram.Extensions.WorkflowExtensions;
using UtilityBills.Telegram.Workflows.AddCredentialWorkflowFeature;
using UtilityBills.Telegram.Workflows.Core.Models;
using UtilityBills.Telegram.Workflows.SendWaterMeterReadingsWorkflowFeature;
using WorkflowCore.Interface;

namespace UtilityBills.Telegram;

public class TelegramHostService : BackgroundService
{
    private static readonly ConcurrentDictionary<string, string> UserIdToWorkflowId = new();

    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IServiceProvider _serviceProvider;

    public TelegramHostService(ITelegramBotClient telegramBotClient, IServiceProvider serviceProvider)
    {
        _telegramBotClient = telegramBotClient;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
    
        await _telegramBotClient.SetMyCommands([
            new BotCommand
            {
                Command = Command.AddCredentialCommand,
                Description = "Add a new credentials from platform"
            },
            new BotCommand
            {
                Command = Command.DeleteCredentialCommand,
                Description = "Delete existed credentials"
            },
            new BotCommand
            {
                Command = Command.SendMeterReadings,
                Description = "Send water meter readings to platforms where credentials were added"
            }
        ], cancellationToken: stoppingToken);

        await _telegramBotClient.ReceiveAsync(async (client, update, arg3) =>
            {
                var message = update.Message?.Text ?? update.CallbackQuery?.Data;
                var messageId = update.Message?.MessageId ?? update.CallbackQuery?.Message?.MessageId!;
                var userId = update.Message?.From?.Id.ToString() ?? update.CallbackQuery?.From.Id.ToString()!;

                using var scope = _serviceProvider.CreateScope();
                var workflowHost = scope.ServiceProvider.GetRequiredService<IWorkflowHost>();
                var userMessage = new UserMessage(message!, messageId.Value);

                if (string.Equals(message, "Выйти", StringComparison.OrdinalIgnoreCase))
                {
                    if (UserIdToWorkflowId.TryRemove(userId, out var workflowId))
                    {
                        await workflowHost.TerminateWorkflow(workflowId);
                        return;
                    }
                }

                switch (message)
                {
                    case Command.AddCredentialCommand:
                        UserIdToWorkflowId.TryAdd(userId, await workflowHost.StartWorkflow(
                            nameof(AddCredentialWorkflow),
                            new AddCredentialWorkflowData { UserId = userId! }));
                        break;
                    case Command.SendMeterReadings:
                        UserIdToWorkflowId.TryAdd(userId,
                            await workflowHost.StartWorkflow(nameof(SendMeterReadingsWorkflow),
                                new SendMeterReadingsWorkflowData { UserId = userId! }));
                        break;
                    default:
                        await workflowHost.PublishUserMessageAsync(update.Type, userId!, userMessage);
                        break;
                }
            },
            (client, exception, arg3) => Task.CompletedTask, cancellationToken: stoppingToken);
    }
}