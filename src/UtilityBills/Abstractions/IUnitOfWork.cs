namespace UtilityBills.Abstractions;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken ct = default);
}