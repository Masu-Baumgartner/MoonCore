using System.Security.Claims;
using MoonCore.Authentication;
using MoonCore.Helpers;

namespace MoonCore.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool HasPermission(this ClaimsPrincipal claimsPrincipal, string permission)
    {
        if (claimsPrincipal is not PermClaimsPrinciple permClaimsPrinciple)
            return false;

        return PermissionHelper.HasPermission(permClaimsPrinciple.Permissions, permission);
    }
    
    public static bool HasPermissions(this ClaimsPrincipal claimsPrincipal, string[] permissions)
    {
        if (claimsPrincipal is not PermClaimsPrinciple permClaimsPrinciple)
            return false;

        foreach (var permission in permissions)
        {
            if (!PermissionHelper.HasPermission(permClaimsPrinciple.Permissions, permission))
                return false;
        }

        return true;
    }

    public static T AsIdentity<T>(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal is not PermClaimsPrinciple permClaimsPrinciple)
            throw new ArgumentException("The ClaimsPrincipal is not a PermClaimsPrinciple");

        return (T)permClaimsPrinciple.IdentityModel;
    }
}