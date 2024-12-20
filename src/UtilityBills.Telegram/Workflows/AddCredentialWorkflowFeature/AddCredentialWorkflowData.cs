using FluentResults;
using Telegram.Bot.Types.ReplyMarkups;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;
using UtilityBills.Telegram.Workflows.Core.Abstractions;

namespace UtilityBills.Telegram.Workflows.AddCredentialWorkflowFeature;

public class AddCredentialWorkflowData : IUserStep
{
    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = [];

    public List<UtilityPaymentPlatform> UtilityPaymentPlatforms { get; set; } = [];

    public Guid PlatformId { get; set; }

    public Result<Email> Email { get; set; } = null!;
    
    public Result<Password> Password { get; set; } = null!;

    public Result<UtilityPaymentPlatformCredential> CreatedCredential { get; set; } = null!;

    public IEnumerable<InlineKeyboardButton> GetButtonsToSelectPlatform()
    {
        return UtilityPaymentPlatforms.Select(platform =>
            InlineKeyboardButton.WithCallbackData(platform.Name, platform.Id.ToString()));
    }
}