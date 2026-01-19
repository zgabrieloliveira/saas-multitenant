using Core.Interfaces;

namespace Core.Common;

/// <summary>
/// base class for all entities that require database persistence and multi-tenancy support
/// </summary>
public abstract class BaseEntity : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; } = Guid.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}