using System.Security.Claims;

namespace MoonCore.Extended.JwtExpire;

public interface IJwtInvalidateHandler
{
    public Task<bool> Handle(ClaimsPrincipal principal);
}