using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TelegramBotCommandFramework.Services;

namespace UtilityBills.Host.BackgroundServices;

public class TelegramHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITelegramBotClient _botClient;

    public TelegramHostedService(IServiceProvider serviceProvider, ITelegramBotClient botClient)
    {
        _serviceProvider = serviceProvider;
        _botClient = botClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var handler = new CommandUpdateHandler(_serviceProvider);

        await _botClient.ReceiveAsync((client, update, arg3) => handler.HandleUpdateAsync(client, update, arg3),
            (_, exception, _) => Console.WriteLine(exception.InnerException?.Message ?? exception.Message),
            new ReceiverOptions { DropPendingUpdates = true, AllowedUpdates = [UpdateType.Message] },
            cancellationToken: stoppingToken);
    }
}