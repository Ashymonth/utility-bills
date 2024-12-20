using Ardalis.Specification;

namespace UtilityBills.Abstractions;

public interface IRepository<TAggregate> : IReadRepositoryBase<TAggregate> where TAggregate : class
{
    IUnitOfWork UnitOfWork { get; }

    Task InsertAsync(TAggregate aggregate, CancellationToken ct = default);

    Task InsertAsync(IEnumerable<TAggregate> aggregate, CancellationToken ct = default);

    Task UpdateAsync(TAggregate aggregate, CancellationToken ct = default);

    Task RemoveAsync(TAggregate aggregate, CancellationToken ct = default);
}