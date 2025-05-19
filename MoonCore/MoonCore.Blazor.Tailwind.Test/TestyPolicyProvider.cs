using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test;

public class TestyPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly string Prefix;

    // Default policy fallback
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

    public TestyPolicyProvider(IOptions<AuthorizationOptions> options, IOptions<PermissionOptions> permissionOptions)
    {
        FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);

        Prefix = permissionOptions.Value.Prefix;
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        FallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        FallbackPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        
        var policy = new AuthorizationPolicyBuilder();
        policy.AddRequirements(new PermissionRequirement(Formatter.ReplaceStart(policyName, Prefix, "")));
        return Task.FromResult<AuthorizationPolicy?>(policy.Build());
    }
}