namespace Core.Interfaces;

/// <summary>
/// marker interface for entities that belong to a specific tenant.
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// the unique identifier of the tenant that owns this entity.
    /// </summary>
    Guid TenantId { get; set; }
}