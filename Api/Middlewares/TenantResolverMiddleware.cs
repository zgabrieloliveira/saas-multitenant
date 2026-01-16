using Core.Interfaces;

namespace Api.Middlewares;

public class TenantResolverMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentTenantService tenantService)
    {
        if (context.Request.Path.StartsWithSegments("/swagger") || 
            context.Request.Path.StartsWithSegments("/index.html"))
        {
            await next(context);
            return;
        }
        
        if (context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantIdValue))
        {
            if (Guid.TryParse(tenantIdValue.ToString(), out Guid parsedTenantId))
            {
                if (tenantService.SetTenantId(parsedTenantId))
                {
                    await next(context);
                    return;
                }
            }
        }
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { error = "Tenant ID missing or invalid format (Expected GUID)" });
    }
}