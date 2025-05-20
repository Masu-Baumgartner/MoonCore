using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace MoonCore.Permissions;

public static class ServiceCollectionExtensions
{
    public static void AddAuthorizationPermissions(
        this IServiceCollection collection,
        Action<PermissionsOptions> onConfigure
    )
    {
        collection.AddSingleton<IAuthorizationPolicyProvider, PermissionsPolicyProvider>();
        collection.AddSingleton<IAuthorizationHandler, PermissionsHandler>();

        collection.Configure(onConfigure);
    }
}