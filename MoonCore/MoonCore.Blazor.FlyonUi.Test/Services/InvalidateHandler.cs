using System.Security.Claims;
using MoonCore.Extended.JwtInvalidation;

namespace MoonCore.Blazor.FlyonUi.Test.Services;

public class InvalidateHandler : IJwtInvalidateHandler
{
    public static DateTimeOffset ExpireTime = DateTimeOffset.UtcNow.AddMinutes(-1);
    
    public Task<bool> Handle(ClaimsPrincipal principal)
    {
        var iatStr = principal.FindFirstValue("iat")!;
        var iat = DateTimeOffset.FromUnixTimeSeconds(long.Parse(iatStr));
        
        Console.WriteLine($"Expire time is {ExpireTime}");
        Console.WriteLine($"Issued at is {iat}");
        
        Console.WriteLine($"Issued at bigger than expire time: {iat > ExpireTime}");
        
        return Task.FromResult(
            ExpireTime > iat
        );
    }
}