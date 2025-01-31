using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MoonCore.Helpers;

namespace MoonCore.Extended.PermFilter;

public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
{
    private string Permission;

    public RequirePermissionAttribute(string permission)
    {
        Permission = permission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!Handle(context))
            context.Result = new ForbidResult();
    }

    private bool Handle(AuthorizationFilterContext context)
    {
        var permissionsClaim = context
            .HttpContext
            .User
            .Claims
            .FirstOrDefault(x => x.Type == "permissions");

        if (permissionsClaim == null)
            return false;

        string[]? permissions = null;

        if (context.HttpContext.Items.TryGetValue("Permissions", out var val))
            permissions = val as string[];

        if (permissions == null)
        {
            permissions = permissionsClaim.Value.Split(";", StringSplitOptions.RemoveEmptyEntries);
            context.HttpContext.Items["Permissions"] = permissions;
        }

        return PermissionHelper.HasPermission(permissions, Permission);
    }
}