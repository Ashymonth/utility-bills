using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using UtilityBills.Abstractions.Services;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;

namespace UtilityBills.Host;

public class TelegramDebtNotificationProvider : IDebtNotificationProvider
{
    private static readonly Dictionary<UtilityPaymentPlatformType, string> _platformToPaymentLinkMap = new()
    {
        [UtilityPaymentPlatformType.Kvado] = "https://cabinet.kvado.ru/accruals",
    };

    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramDebtNotificationProvider(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public async Task NotifyAboutDebtAsync(string userId, decimal debt, UtilityPaymentPlatformType platform,
        CancellationToken ct = default)
    {
        var paymentLink = _platformToPaymentLinkMap.GetValueOrDefault(platform);
        
        await _telegramBotClient.SendTextMessageAsync(long.Parse(userId),
            $"You have debt: {debt} on platform: {platform}",
            replyMarkup: paymentLink is not null
                ? new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Оплатить", paymentLink))
                : new ReplyKeyboardRemove(),
            cancellationToken: ct);
    }
}