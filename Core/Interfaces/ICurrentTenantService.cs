namespace Core.Interfaces;

public interface ICurrentTenantService
{
    Guid? CurrentTenantId { get; }
    bool SetTenantId(Guid tenantId);
}