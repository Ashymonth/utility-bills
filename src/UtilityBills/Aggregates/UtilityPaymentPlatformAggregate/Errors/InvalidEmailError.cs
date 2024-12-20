using FluentResults;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Errors;

public class InvalidEmailError(string message) : Error(message);