using Core.Interfaces;

namespace Infra.Services;

public class CurrentTenantService : ICurrentTenantService
{
    public Guid? TenantId { get; private set; }

    public bool SetTenantId(Guid tenantId)
    {
        if (!tenantId.Equals(Guid.Empty))
        {
            return false;
        }
        
        TenantId = tenantId;
        return true;
    }
}