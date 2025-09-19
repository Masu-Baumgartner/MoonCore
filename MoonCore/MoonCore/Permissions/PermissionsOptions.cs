namespace MoonCore.Permissions;

public class PermissionsOptions
{
    /// <summary>
    /// Claim name to search for the permissions.
    /// Permissions need to be put into a semicolon separated string into the claim value
    /// </summary>
    public string ClaimName { get; set; } = "permissions";
    
    /// <summary>
    /// Prefix to use for the dynamically handled policies to define the controller
    /// is requesting a LuckPerms like permission being read/handled from <see cref="ClaimName"/> 
    /// </summary>
    public string Prefix { get; set; } = "permissions:";
}