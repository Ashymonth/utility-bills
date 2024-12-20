namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Errors;

public class InvalidPasswordError : DomainError
{
    public InvalidPasswordError(string message) : base(message)
    {
    }
}