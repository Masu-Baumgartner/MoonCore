using System.Reflection;

namespace MoonCore.Blazor.Bootstrap.Models.FastForms;

public interface IFastFormPropertyConfiguration
{
    public List<IFastFormValidator> Validators { get; set; }
    public FastFormPageConfiguration? PageConfiguration { get; set; }
    public FastFormSectionConfiguration? SectionConfiguration { get; set; }
    public PropertyInfo PropertyInfo { get; set; }
    public Type ComponentType { get; set; }
    public object? OnConfigureFunc { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}