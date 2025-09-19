using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MoonCore.Helpers;

namespace MoonCore.Permissions;

/// <summary>
/// Implements an <see cref="AuthorizationHandler{TRequirement}"/> to handle LuckPerms specified permissions
/// from the configured claim
/// </summary>
public class PermissionsHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly PermissionsOptions Options;

    public PermissionsHandler(IOptions<PermissionsOptions> options)
    {
        Options = options.Value;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement
    )
    {
        var claim = context
            .User
            .Claims
            .FirstOrDefault(x => x.Type == Options.ClaimName);

        if (claim == null)
        {
            context.Fail(
                new AuthorizationFailureReason(this, "Permission claim is missing")
            );

            return Task.CompletedTask;
        }

        var permissions = claim.Value.Split(";", StringSplitOptions.RemoveEmptyEntries);

        if (permissions.Length == 0)
        {
            context.Fail(
                new AuthorizationFailureReason(this, "No permissions found")
            );

            return Task.CompletedTask;
        }

        if (PermissionHelper.HasPermission(permissions, requirement.Permission))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        context.Fail(
            new AuthorizationFailureReason(this, "Permission denied")
        );

        return Task.CompletedTask;
    }
}