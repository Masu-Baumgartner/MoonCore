using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Tailwind.LegacyForms;

public struct CompiledFieldConfiguration
{
    public string Label;
    public string? Description;
    public object? DefaultValue;
    public ComponentBase Component;
    public int Columns;
    public PropertyInfo Property;
}