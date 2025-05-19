using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test;

public class PermissionClaimHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly string ClaimName;

    public PermissionClaimHandler(IOptions<PermissionOptions> options)
    {
        ClaimName = options.Value.ClaimName;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement
    )
    {
        var claim = context.User.Claims.FirstOrDefault(x => x.Type == ClaimName);

        if (claim == null)
        {
            Console.WriteLine("Missing permission claim");
            
            context.Fail(new AuthorizationFailureReason(
                this,
                "Missing permission claim")
            );

            return Task.CompletedTask;
        }

        var permissions = claim.Value.Split(";", StringSplitOptions.RemoveEmptyEntries);

        if (!PermissionHelper.HasPermission(permissions, requirement.Permission))
        {
            Console.WriteLine("Missing permission");
            
            context.Fail(new AuthorizationFailureReason(
                this,
                "Missing permission")
            );
            
            return Task.CompletedTask;
        }
        
        context.Succeed(requirement);
        
        return Task.CompletedTask;
    }
}