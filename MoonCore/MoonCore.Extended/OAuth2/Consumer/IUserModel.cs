namespace MoonCore.Extended.OAuth2.Consumer;

public interface IUserModel
{
    public int Id { get; set; }

    public DateTime RefreshTimestamp { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}