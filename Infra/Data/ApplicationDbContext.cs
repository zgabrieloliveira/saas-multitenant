using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infra.Data;

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
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

    private void ConfigureGlobalFilters<T>(ModelBuilder builder) 
        where T : class, ITenantEntity
    {
        builder.Entity<T>().HasQueryFilter(e => e.TenantId == _tenantService.CurrentTenantId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
            {
                // Usa o campo privado aqui também
                entry.Entity.TenantId = _tenantService.CurrentTenantId 
                    ?? throw new InvalidOperationException("Tenant não identificado!");
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}