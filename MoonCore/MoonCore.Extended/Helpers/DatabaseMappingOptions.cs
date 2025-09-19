namespace MoonCore.Extended.Helpers;

/// <summary>
/// Container for database mappings
/// </summary>
public class DatabaseMappingOptions
{
    public readonly Dictionary<Type, Type> Mappings = new();
}