using Microsoft.AspNetCore.Authorization;

namespace MoonCore.Permissions;

/// <summary>
/// Defines the requirement of a specific permission to be granted to the user
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// LuckPerms like permission string
    /// </summary>
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}