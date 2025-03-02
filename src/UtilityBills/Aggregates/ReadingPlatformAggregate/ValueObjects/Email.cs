using System.Net.Mail;
using FluentResults;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Errors;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;

public record Email
{
    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; private set; }

    public static Result<Email> Create(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        if (!MailAddress.TryCreate(email, out _))
        {
            return new InvalidEmailError("Provided string is not valid email");
        }

        return new Email(email);
    }
}