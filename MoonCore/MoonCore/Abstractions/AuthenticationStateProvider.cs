using MoonCore.Helpers;

namespace MoonCore.Abstractions;

public abstract class AuthenticationStateProvider
{
    public abstract Task<bool> IsValidIdentifier(IServiceProvider provider, string identifier);
    public abstract Task LoadFromIdentifier(IServiceProvider provider, string identifier, DynamicStorage storage);
    public abstract Task<DateTime> DetermineTokenValidTimestamp(IServiceProvider provider, string identifier);
}