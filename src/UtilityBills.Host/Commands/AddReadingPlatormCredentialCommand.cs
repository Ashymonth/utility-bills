using InvestManager.Host.Telegram.Abstractions;
using InvestManager.Host.Telegram.Attributes;
using InvestManager.Host.Telegram.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Services;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;
using UtilityBills.Telegram;

namespace UtilityBills.Host.Commands;

[TelegramCommand(Command.AddCredentialCommand)]
public class AddReadingPlatformCredentialCommand : ITelegramCommand
{
    private readonly IReadingPlatformService _platformService;
    private readonly IPasswordProtector _passwordProtector;

    public AddReadingPlatformCredentialCommand(IReadingPlatformService platformService,
        IPasswordProtector passwordProtector)
    {
        _platformService = platformService;
        _passwordProtector = passwordProtector;
    }

    public async Task ExecuteAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        var userId = update.GetUserId();
        await bot.SendMessage(userId, "Идет загрузка списка платформ...", cancellationToken: ct);

        var platforms = (await _platformService.GetPlatformsAsync(ct))
            .ToDictionary(readingPlatform => readingPlatform.Name.ToLower());

        var selectedPlatform = await RequestSelectPlatform(userId, platforms, bot, ct);

        var userEmail = await RequestSelectEmail(userId, bot, ct);
 
        var userPassword =  await RequestSelectPassword(userId, bot, ct);

        await bot.SendMessage(userId, "Попытка авторизоваться на платформе с указанным логином и паролем. Ожидайте...",
            cancellationToken: ct);

        var result = await _platformService.AddCredentialToPlatformAsync(selectedPlatform.Id, userEmail, userPassword,
            userId.ToString(), ct);

        if (result.IsSuccess)
        {
            await bot.SendMessage(userId,
                $"Вы успешно авторизовались на платформе: {selectedPlatform.Name}\n" +
                $"Почта: {userEmail.Value}.",
                cancellationToken: ct);
            return;
        }

        await bot.SendMessage(userId, "Не удалось авторизоваться. Проверьте логи и пароль и повторите попытку",
            cancellationToken: ct);
    }

    private static async Task<ReadingPlatform> RequestSelectPlatform(long userId,
        Dictionary<string, ReadingPlatform> platforms,
        ITelegramBotClient bot, CancellationToken ct)
    {
        var platformNames = new ReplyKeyboardMarkup(platforms.Values.Select(p => new KeyboardButton(p.Name)).ToArray())
        {
            ResizeKeyboard = true,
        };

        ReadingPlatform? selectedPlatform = null;

        while (selectedPlatform is null)
        {
            await bot.SendMessage(userId, "Выберите платформу для авторизации", replyMarkup: platformNames,
                cancellationToken: ct);

            var selectedPlatformName = (await bot.WaitForUserMessageAsync(userId))?.Text;

            selectedPlatform = platforms!.GetValueOrDefault(selectedPlatformName?.ToLower());
        }

        return selectedPlatform;
    }

    private static async Task<Email> RequestSelectEmail(long userId, ITelegramBotClient bot, CancellationToken ct)
    {
        Email userEmail = null!;

        while (userEmail is null)
        {
            await bot.SendMessage(userId, "Введите email ", replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: ct);

            var emailResponse = await bot.WaitForUserMessageAsync(userId);

            userEmail = Email.Create(emailResponse!.Text!).ValueOrDefault;
        }

        return userEmail;
    }

    private async Task<Password> RequestSelectPassword(long userId, ITelegramBotClient bot, CancellationToken ct)
    {
        Password userPassword = null!;
        while (userPassword is null)
        {
            await bot.SendMessage(userId, "Введите пароль ", replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: ct);

            var passwordResponse = await bot.WaitForUserMessageAsync(userId);

            userPassword = Password.Create(passwordResponse!.Text!, _passwordProtector).ValueOrDefault;
        }

        return userPassword;
    }
}