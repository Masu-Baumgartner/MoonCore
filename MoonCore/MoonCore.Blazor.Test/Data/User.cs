namespace MoonCore.Blazor.Test.Data;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime TokenValidTimestamp { get; set; } = DateTime.MinValue;
}