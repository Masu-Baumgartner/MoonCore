namespace MoonCore.Extended.OAuth2.Consumer;

public interface IDataProvider<T> where T : IUserModel
{
    public Task SaveChanges(T model);
    public Task<T?> LoadById(int id);
}