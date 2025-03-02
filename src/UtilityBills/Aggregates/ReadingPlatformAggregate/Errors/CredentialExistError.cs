using FluentResults;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate.Errors;

public class CredentialExistError : DomainError
{
    public CredentialExistError(string message) : base(message)
    {
    }
}