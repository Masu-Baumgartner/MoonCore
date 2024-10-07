namespace MoonCore.Blazor.Tailwind.Attributes;

public class RequirePermissionAttribute : Attribute
{
    public string Permission { get; set; }
    
    public RequirePermissionAttribute(string permission)
    {
        Permission = permission;
    }
}