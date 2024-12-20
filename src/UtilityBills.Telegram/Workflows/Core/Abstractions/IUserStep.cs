namespace UtilityBills.Telegram.Workflows.Core.Abstractions;

public interface IUserStep
{
    string UserId { get; set; }
    
    List<int> SentMessageIds { get; set; }
}