using MoonCore.Extended.OAuth2.Consumer;

namespace MoonCore.Extended.OAuth2.LocalProvider;

public interface ILocalProviderImplementation<T> : IDataProvider<T> where T : IUserModel
{
    public Task<T> Login(string email, string password);
    public Task<T> Register(string username, string email, string password);
}