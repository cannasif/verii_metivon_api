using Microsoft.AspNetCore.Authorization;
using verii_metivon_api.Modules.AccessControl.Authorization;

namespace verii_metivon_api.Modules.AccessControl;

public static class AccessControlModule
{
    public static IServiceCollection AddAccessControlModule(this IServiceCollection services)
    {
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddScoped<IAccessControlAuditWriter, AccessControlAuditWriter>();
        services.AddScoped<ErpPermissionAuthorizationFilter>();
        return services;
    }
}
