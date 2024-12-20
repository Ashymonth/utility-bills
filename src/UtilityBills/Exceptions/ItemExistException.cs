namespace UtilityBills.Exceptions;

public class ItemExistException : DomainException
{
    public ItemExistException(string message) : base(message)
    {
    }
}