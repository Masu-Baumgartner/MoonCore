using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace MoonCore.Permissions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a LuckPerms like permission string policy provider and handler
    /// </summary>
    /// <param name="collection">Collection to add the services to</param>
    /// <param name="onConfigure">Callback to configure options of the services</param>
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