using MoonCore.Extended.OAuth2.Models;

namespace MoonCore.Extended.Interfaces;

public interface IOAuth2Provider
{
    public Task<Dictionary<string, object>> Sync(IServiceProvider serviceProvider, AccessData accessData);
}