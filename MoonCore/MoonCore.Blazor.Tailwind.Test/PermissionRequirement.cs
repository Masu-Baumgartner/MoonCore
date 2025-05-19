using Microsoft.AspNetCore.Authorization;

namespace MoonCore.Blazor.Tailwind.Test;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}