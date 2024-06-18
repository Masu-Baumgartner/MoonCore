using MoonCore.Abstractions;
using MoonCore.Blazor.Test.Data;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Test;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    public static List<User> Users = new()
    {
        new()
        {
            Id = 1,
            Email = "admin@masuowo.xyz",
            Password = "12345678"
        },
        new()
        {
            Id = 2,
            Email = "admin2@masuowo.xyz",
            Password = "12345678"
        }
    };
    public override async Task<bool> IsValidIdentifier(IServiceProvider provider, string identifier)
    {
        var id = int.Parse(identifier);

        if (Users.Any(x => x.Id == id))
            return true;

        return false;
    }

    public override async Task LoadFromIdentifier(IServiceProvider provider, string identifier, DynamicStorage storage)
    {
        var id = int.Parse(identifier);
        var user = Users.First(x => x.Id == id);
        
        storage.Set("User", user);
    }

    public override async Task<DateTime> DetermineTokenValidTimestamp(IServiceProvider provider, string identifier)
    {
        var id = int.Parse(identifier);
        var user = Users.First(x => x.Id == id);

        return user.TokenValidTimestamp;
    }
}