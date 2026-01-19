using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infra.Data;

/// <summary>
/// primary database context handling multi-tenancy logic and persistence
/// </summary>
public class ApplicationDbContext : DbContext
{
    private readonly ICurrentTenantService _tenantService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentTenantService tenantService) : base(options)
    {
        _tenantService = tenantService;
    }

    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// configures the schema and applies global query filters for multi-tenancy
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // automatically finds all entities implementing itenantentity and applies the filter
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(entityType.ClrType);
                method?.Invoke(this, [builder]);
            }
        }
    }

    /// <summary>
    /// helper method to apply the tenant filter expression dynamically
    /// </summary>
    private void ConfigureGlobalFilters<T>(ModelBuilder builder) 
        where T : class, ITenantEntity
    {
        // ef core reads the property from the service on every query
        builder.Entity<T>().HasQueryFilter(e => e.TenantId == _tenantService.CurrentTenantId);
    }

    /// <summary>
    /// saves changes to the database, automatically enforcing tenant isolation on insert
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
            {
                // ensures no entity is created without a tenant owner
                entry.Entity.TenantId = _tenantService.CurrentTenantId 
                    ?? throw new InvalidOperationException("tenant could not be identified!");
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}