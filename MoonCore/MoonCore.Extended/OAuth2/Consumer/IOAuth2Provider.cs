using MoonCore.Models;

namespace MoonCore.Extended.OAuth2.Consumer;

public interface IOAuth2Provider<T> where T : IUserModel
{
    public Task<TokenPair> ProcessAccess(string code);
    public Task<TokenPair> ProcessRefresh(string refreshToken);
    public Task<T> ProcessSync(string accessToken);
}