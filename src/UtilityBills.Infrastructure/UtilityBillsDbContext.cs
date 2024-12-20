using Microsoft.EntityFrameworkCore;
using UtilityBills.Abstractions;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities;
using UtilityBills.Entities;

namespace UtilityBills.Infrastructure;

public class UtilityBillsDbContext : DbContext, IUnitOfWork
{
    public UtilityBillsDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbSet<User> Users => Set<User>();

    public DbSet<UtilityPaymentPlatform> UtilityPaymentPlatforms => Set<UtilityPaymentPlatform>();

    public DbSet<UtilityPaymentPlatformCredential> UtilityPaymentPlatformCredentials =>
        Set<UtilityPaymentPlatformCredential>();

    public async Task CommitAsync(CancellationToken ct = default)
    {
        await SaveChangesAsync(ct);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UtilityBillsDbContext).Assembly);
    }
}