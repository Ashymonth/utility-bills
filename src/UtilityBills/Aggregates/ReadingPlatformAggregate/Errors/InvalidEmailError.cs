using FluentResults;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate.Errors;

public class InvalidEmailError(string message) : Error(message);