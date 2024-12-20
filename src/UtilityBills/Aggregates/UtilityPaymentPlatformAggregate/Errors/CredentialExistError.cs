using FluentResults;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Errors;

public class CredentialExistError : DomainError
{
    public CredentialExistError(string message) : base(message)
    {
    }
}