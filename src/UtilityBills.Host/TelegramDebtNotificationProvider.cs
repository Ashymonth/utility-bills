using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using UtilityBills.Abstractions.Services;
using UtilityBills.Aggregates.ReadingPlatformAggregate;

namespace UtilityBills.Host;

public class TelegramDebtNotificationProvider : IDebtNotificationProvider
{
    private static readonly Dictionary<ReadingPlatformType, string> _platformToPaymentLinkMap = new()
    {
        [ReadingPlatformType.Kvado] = "https://cabinet.kvado.ru/accruals",
    };

    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramDebtNotificationProvider(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public async Task NotifyAboutDebtAsync(string userId, decimal debt, ReadingPlatformType platform,
        CancellationToken ct = default)
    {
        var paymentLink = _platformToPaymentLinkMap.GetValueOrDefault(platform);
        
        await _telegramBotClient.SendMessage(long.Parse(userId),
            $"You have debt: {debt} on platform: {platform}",
            replyMarkup: paymentLink is not null
                ? new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Оплатить", paymentLink))
                : new ReplyKeyboardRemove(),
            cancellationToken: ct);
    }
}