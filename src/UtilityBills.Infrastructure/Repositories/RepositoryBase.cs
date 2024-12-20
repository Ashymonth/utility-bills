using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using UtilityBills.Abstractions;

namespace UtilityBills.Infrastructure.Repositories;

public class EfRepository<TAggregate> : RepositoryBase<TAggregate>, IRepository<TAggregate> where TAggregate : class
{
    private readonly UtilityBillsDbContext _dbContext;

    public EfRepository(UtilityBillsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public EfRepository(UtilityBillsDbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext,
        specificationEvaluator)
    {
        _dbContext = dbContext;
    }

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task InsertAsync(TAggregate aggregate, CancellationToken ct = default)
    {
        await _dbContext.Set<TAggregate>().AddAsync(aggregate, ct);
    }

    public async Task InsertAsync(IEnumerable<TAggregate> aggregate, CancellationToken ct = default)
    {
        await _dbContext.Set<TAggregate>().AddRangeAsync(aggregate, ct);
    }

    public new Task UpdateAsync(TAggregate aggregate, CancellationToken ct = default)
    {
        _dbContext.Set<TAggregate>().Update(aggregate);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(TAggregate aggregate, CancellationToken ct = default)
    {
        _dbContext.Remove(aggregate);
        return Task.CompletedTask;
    }
}