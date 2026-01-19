namespace Core.Interfaces;

/// <summary>
/// defines the contract for managing the current tenant context
/// </summary>
public interface ICurrentTenantService
{
    /// <summary>
    /// gets the current tenant id if resolved, otherwise null
    /// </summary>
    Guid? CurrentTenantId { get; }

    /// <summary>
    /// attempts to set the tenant id for the current request scope.
    /// </summary>
    /// <param name="tenantId">the unique identifier of the tenant.</param>
    /// <returns>true if the tenant was set successfully; false if already set or invalid.</returns>
    bool SetTenantId(Guid tenantId);
}