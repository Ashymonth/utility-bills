using InvestManager.Host.Telegram.Extensions;
using InvestManager.Host.Telegram.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace UtilityBills.Host.Commands;

public class ExitTelegramCommandInterceptor : ITelegramCommandInterceptor
{
    private const string ExitCommand = "выйти";

    public async Task<bool> BeforeUpdateExecutedAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (!string.Equals(update.Message?.Text, ExitCommand, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        await botClient.SendMessage(update.GetUserId(), "Команда отменена", cancellationToken: ct);
        return false;
    }

    public Task AfterUpdateExecutedAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}