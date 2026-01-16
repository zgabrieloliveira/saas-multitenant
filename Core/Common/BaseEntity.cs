using Core.Interfaces;

namespace Core.Common;

public abstract class BaseEntity : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; } =  Guid.Empty;
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
}