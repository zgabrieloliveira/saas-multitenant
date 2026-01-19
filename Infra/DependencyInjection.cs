using Core.Interfaces;
using Infra.Services;
using Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra;

/// <summary>
/// extension methods to register infrastructure services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// registers database context and scoped services
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // postgres config
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // SCOPED: different HTTP request = different contexts
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();

        return services;
    }
}