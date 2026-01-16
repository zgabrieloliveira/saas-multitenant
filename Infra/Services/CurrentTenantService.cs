using Core.Interfaces;

namespace Infra.Services;

public class CurrentTenantService : ICurrentTenantService
{
    public Guid? CurrentTenantId { get; private set; }

    public bool SetTenantId(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            return false;
        }
        
        if (CurrentTenantId.HasValue) 
        {
            return false;
        }
    
        CurrentTenantId = tenantId;
        return true;
    }
}