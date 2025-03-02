using Microsoft.EntityFrameworkCore;
using UtilityBills.Abstractions;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;
using UtilityBills.Entities;

namespace UtilityBills.Infrastructure;

public class UtilityBillsDbContext : DbContext, IUnitOfWork
{
    public UtilityBillsDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbSet<User> Users => Set<User>();

    public DbSet<ReadingPlatform> ReadingPlatforms => Set<ReadingPlatform>();

    public DbSet<ReadingPlatformCredential> ReadingPlatformCredentials =>
        Set<ReadingPlatformCredential>();

    public async Task CommitAsync(CancellationToken ct = default)
    {
        await SaveChangesAsync(ct);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UtilityBillsDbContext).Assembly);
    }
}