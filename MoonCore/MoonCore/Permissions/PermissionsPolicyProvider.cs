using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MoonCore.Helpers;

namespace MoonCore.Permissions;

/// <summary>
/// Implementation of the <see cref="IAuthorizationPolicyProvider"/> to provide
/// access to policies based on a LuckPerms like permssion string system when a specific
/// prefix is provided in the requested policy name
/// </summary>
public class PermissionsPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly PermissionsOptions Options;
    private readonly IAuthorizationPolicyProvider FallbackProvider;

    public PermissionsPolicyProvider(IOptions<AuthorizationOptions> authOptions, IOptions<PermissionsOptions> options)
    {
        FallbackProvider = new DefaultAuthorizationPolicyProvider(authOptions);
        Options = options.Value;
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(Options.Prefix, StringComparison.InvariantCultureIgnoreCase))
            return FallbackProvider.GetPolicyAsync(policyName);

        var permission = Formatter.ReplaceStart(policyName, Options.Prefix, "");

        var policy = new AuthorizationPolicyBuilder();

        policy.AddRequirements(
            new PermissionRequirement(permission)
        );

        return Task.FromResult<AuthorizationPolicy?>(
            policy.Build()
        );
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        FallbackProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        FallbackProvider.GetFallbackPolicyAsync();
}