using System.Reflection;
using Microsoft.AspNetCore.Components;
using MoonCore.Blazor.Helpers;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.LegacyForms;

public abstract class BaseFieldConfiguration
{
    public string? Label { get; set; }
    public string? Description { get; set; }
    public object? DefaultValue { get; set; }
    public Type? ComponentType { get; set; }
    public object? ComponentConfigureAction { get; set; }
    public int Columns { get; set; } = 2;
    public PropertyInfo Property { get; set; }

    public CompiledFieldConfiguration Compile(IServiceProvider provider, object model)
    {
        return new CompiledFieldConfiguration()
        {
            Columns = Columns,
            Description = Description,
            DefaultValue = DefaultValue,
            Label = Label ?? Formatter.ConvertCamelCaseToSpaces(Property.Name),
            Property = Property,
            Component = ConstructComponent(provider, model)
        };
    }

    private ComponentBase ConstructComponent(IServiceProvider provider, object model)
    {
        // determin component type
        Type componentType;
        
        if (ComponentType == null)
        {
            var componentT = FormComponentRepository.Get(Property.PropertyType);

            if (componentT == null)
                throw new ArgumentException(
                    $"Unable to compile field with auto selected component type as no component has been registered for '{Property.PropertyType.Name}'");

            componentType = componentT;
        }
        else
            componentType = ComponentType;

        // Create component
        var component = (Activator.CreateInstance(componentType) as ComponentBase)!;
        
        // Create property binder
        var propBinderType = typeof(PropRefBinder<>).MakeGenericType(Property.PropertyType);
        var propBinder = Activator.CreateInstance(propBinderType, [model, Property])!;
        
        // Set properties of the component
        componentType.GetProperty("Binder")!.SetValue(component, propBinder);
        componentType.GetProperty("ServiceProvider")!.SetValue(component, provider);
        
        // ^
        // We need to pass the service provider here, as injected services won't work in the instance view component :c
        // this only affects the single component in the instance view, not all components below
        
        // Call manual configure if required
        if (ComponentConfigureAction != null)
        {
            ComponentConfigureAction
                .GetType()
                .GetMethod("Invoke")!
                .Invoke(ComponentConfigureAction, [component]);
        }

        // Set default value if specified
        if (DefaultValue != null)
            Property.SetValue(model, DefaultValue);

        return component;
    }
}