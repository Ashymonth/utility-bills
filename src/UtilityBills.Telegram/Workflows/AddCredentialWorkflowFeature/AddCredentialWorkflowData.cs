using FluentResults;
using Telegram.Bot.Types.ReplyMarkups;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;
using UtilityBills.Telegram.Workflows.Core.Abstractions;

namespace UtilityBills.Telegram.Workflows.AddCredentialWorkflowFeature;

public class AddCredentialWorkflowData : IUserStep
{
    public string UserId { get; set; } = null!;

    public List<int> SentMessageIds { get; set; } = [];

    public List<ReadingPlatform> ReadingPlatforms { get; set; } = [];

    public Guid PlatformId { get; set; }

    public Result<Email> Email { get; set; } = null!;
    
    public Result<Password> Password { get; set; } = null!;

    public Result<ReadingPlatformCredential> CreatedCredential { get; set; } = null!;

    public IEnumerable<InlineKeyboardButton> GetButtonsToSelectPlatform()
    {
        return ReadingPlatforms.Select(platform =>
            InlineKeyboardButton.WithCallbackData(platform.Name, platform.Id.ToString()));
    }
}