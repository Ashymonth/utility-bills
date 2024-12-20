namespace UtilityBills.Telegram.Workflows.Core.Abstractions;

public interface IFinalStep
{
    string? ErrorMessage { get; set; }
}