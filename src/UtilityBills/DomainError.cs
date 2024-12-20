using FluentResults;

namespace UtilityBills;

public class DomainError : Error
{
    public DomainError(string message) : base(message)
    {
        
    }
}