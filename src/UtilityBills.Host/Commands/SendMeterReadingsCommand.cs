using InvestManager.Host.Telegram.Abstractions;
using InvestManager.Host.Telegram.Attributes;
using InvestManager.Host.Telegram.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UtilityBills.Abstractions;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Services;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;
using UtilityBills.Telegram;

namespace UtilityBills.Host.Commands;

[TelegramCommand(Command.SendMeterReadings)]
public class SendMeterReadingsCommand : ITelegramCommand
{
    private readonly IRepository<ReadingPlatformCredential> _readingPlatformCredentialRepository;
    private readonly IMeterReadingsService _readingsService;

    public SendMeterReadingsCommand(IRepository<ReadingPlatformCredential> readingPlatformCredentialRepository,
        IMeterReadingsService readingsService)
    {
        _readingPlatformCredentialRepository = readingPlatformCredentialRepository;
        _readingsService = readingsService;
    }

    public async Task ExecuteAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        var userId = update.GetUserId();
        await bot.SendMessage(userId, "Загрузка ваших доступов...", cancellationToken: ct);

        var userCredentials = await _readingPlatformCredentialRepository.ListAsync(ct);

        if (userCredentials.Count == 0)
        {
            await bot.SendMessage(userId, "Сначала нужно добавить хотя бы один логин и пароль для платформы",
                cancellationToken: ct);
            return;
        }

        await bot.SendMessage(userId, "Получение прошлых показаний по счетчикам...", cancellationToken: ct);

        var previousMeterReadings = await _readingsService.GetPreviousReadingsAsync(userId.ToString(), ct);

        if (previousMeterReadings.IsFailed)
        {
            await bot.SendMessage(userId, "Не удалось получить прошлые показания. Попробуйте позже.",
                cancellationToken: ct);
            return;
        }

        await bot.SendMessage(userId, "Прошлые показания:\n" +
                                      $"Горячая вода: {previousMeterReadings.Value.HotWater.Value}\n" +
                                      $"Холодная вода: {previousMeterReadings.Value.ColdWater!.Value}",
            cancellationToken: ct);

        var userHotWater = await RequestWaterMeterReadingsAsync(userId, previousMeterReadings.Value.HotWater,
            "Введите показания <b>горячей</b> воды\n" +
            $"Предыдущие показания: {previousMeterReadings.Value.HotWater.Value}", bot, ct);

        var userColdWater = await RequestWaterMeterReadingsAsync(userId, previousMeterReadings.Value.HotWater,
            "Введите показания <b>холодной</b> воды\n" +
            $"Предыдущие показания: {previousMeterReadings.Value.ColdWater.Value}", bot, ct);
        
        await bot.SendMessage(userId, "Передача показаний. Ожидайте...", cancellationToken: ct);

        var result = await _readingsService.SendReadingsAsync(userId.ToString(), userHotWater, userColdWater, ct);

        if (result.IsSuccess)
        {
            await bot.SendMessage(userId, "Показания успешно отправлены", cancellationToken: ct);
            return;
        }

        await bot.SendMessage(userId, "Не удалось отправить показания. Попробуйте позже", cancellationToken: ct);
    }

    private static async Task<MeterReadings> RequestWaterMeterReadingsAsync(long userId,
        MeterReadings previousMeterReadings,
        string requestMeterReadings,
        ITelegramBotClient bot,
        CancellationToken ct)
    {
        MeterReadings? hotWater = null;

        while (hotWater is null)
        {
            await bot.SendMessage(userId, requestMeterReadings, ParseMode.Html, cancellationToken: ct);

            var userHotWater = await bot.WaitForUserMessageAsync(userId);
            if (!int.TryParse(userHotWater!.Text, out var userHotWaterNumber))
            {
                await bot.SendMessage(userId, "Показания должны быть целым числом", cancellationToken: ct);
                continue;
            }

            if (userHotWaterNumber < previousMeterReadings.Value)
            {
                await bot.SendMessage(userId, "Показания не могут быть меньше, чем переданные в прошлый раз",
                    cancellationToken: ct);
                continue;
            }

            hotWater = MeterReadings.Create(userHotWaterNumber).ValueOrDefault;
        }

        return hotWater;
    }
}