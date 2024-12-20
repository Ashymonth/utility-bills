namespace UtilityBills.Abstractions;

public class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    
    public Guid Id { get; set; }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}