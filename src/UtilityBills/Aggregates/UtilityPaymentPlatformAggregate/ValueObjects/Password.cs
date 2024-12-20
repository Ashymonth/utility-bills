using FluentResults;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

public record Password
{
    private string Value { get;  set; }
    
    private Password(string value)
    {
        Value = value;
    }

    public string GetUnprotected(IPasswordProtector passwordProtector)
    {
        return passwordProtector.Unprotect(Value);
    }

    public static Result<Password> Create(string password, IPasswordProtector passwordProtector)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return Result.Fail("Password can't be empty");
        }

        return new Password(passwordProtector.Protect(password));
    }
}