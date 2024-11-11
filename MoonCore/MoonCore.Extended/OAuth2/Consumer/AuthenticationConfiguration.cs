namespace MoonCore.Extended.OAuth2.Consumer;

public class AuthenticationConfiguration<T> where T : IUserModel
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RedirectUri { get; set; }
    public string AuthorizeEndpoint { get; set; }
    public string AccessSecret { get; set; }
    public string RefreshSecret { get; set; }
    public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan RefreshDuration { get; set; } = TimeSpan.FromDays(10);
}