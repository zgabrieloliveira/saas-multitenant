namespace Core.Interfaces;

public interface ICurrentTenantService
{
    Guid? TenantId { get; }
    bool SetTenantId(Guid tenantId);
}