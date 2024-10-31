using System.Security.Claims;

namespace MoonCore.Authentication;

public class PermClaimsPrinciple : ClaimsPrincipal
{
    public string[] Permissions { get; private set; }
    public object IdentityModel { get; set; }
    
    public PermClaimsPrinciple(string[] permissions)
    {
        Permissions = permissions;
    }
}