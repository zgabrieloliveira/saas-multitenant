using Core.Interfaces;

namespace Infra.Services;

/// <summary>
/// implementation of the tenant service using scoped lifetime (per HTTP request)
/// </summary>
public class CurrentTenantService : ICurrentTenantService
{
    /// <summary>
    /// holds the current tenant id for the request scope
    /// </summary>
    public Guid? CurrentTenantId { get; private set; }

    /// <summary>
    /// sets the tenant id safely, preventing overwrites during the same request.
    /// </summary>
    public bool SetTenantId(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            return false;
        }
        
        // security check: if already set, do not allow change (prevents header injection attacks mid-request)
        if (CurrentTenantId.HasValue) 
        {
            return false;
        }
    
        CurrentTenantId = tenantId;
        return true;
    }
}