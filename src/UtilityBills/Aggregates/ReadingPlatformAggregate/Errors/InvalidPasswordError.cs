namespace UtilityBills.Aggregates.ReadingPlatformAggregate.Errors;

public class InvalidPasswordError : DomainError
{
    public InvalidPasswordError(string message) : base(message)
    {
    }
}