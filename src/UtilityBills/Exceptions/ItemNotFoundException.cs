namespace UtilityBills.Exceptions;

public class ItemNotFoundException : DomainException
{
    public ItemNotFoundException(string message) : base(message)
    {
    }
}