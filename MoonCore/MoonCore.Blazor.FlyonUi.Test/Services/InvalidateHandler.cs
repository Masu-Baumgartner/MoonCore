using System.Security.Claims;
using MoonCore.Extended.JwtExpire;

namespace MoonCore.Blazor.FlyonUi.Test.Services;

public class InvalidateHandler : IJwtInvalidateHandler
{
    public static DateTimeOffset ExpireTime = DateTimeOffset.UtcNow.AddSeconds(-10);
    
    public Task<bool> Handle(ClaimsPrincipal principal)
    {
        var iatStr = principal.FindFirstValue("iat")!;
        var iat = DateTimeOffset.FromUnixTimeSeconds(long.Parse(iatStr));
        
        return Task.FromResult(
            iat > ExpireTime
        );
    }
}