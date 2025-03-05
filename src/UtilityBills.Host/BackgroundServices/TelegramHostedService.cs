using InvestManager.Host.Telegram.Services;
using Telegram.Bot;

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

        await _botClient.ReceiveAsync(handler, cancellationToken: stoppingToken);
    }
}