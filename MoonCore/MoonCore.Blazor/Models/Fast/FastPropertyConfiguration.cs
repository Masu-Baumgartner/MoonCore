using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using MoonCore.Blazor.Forms.Fast.Components;
using MoonCore.Blazor.Helpers;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Models.Fast;

public class FastPropertyConfiguration<T> where T : class
{
    public Expression<Func<T, object?>> Field { get; set; }
    public Type ComponentType { get; set; }
    public List<IFastValidator> Validators { get; set; } = new();
    public FastSectionConfiguration? SectionConfiguration { get; set; }
    public FastPageConfiguration? PageConfiguration { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public DynamicStorage? AdditionalOptions { get; set; }

    public FastPropertyConfiguration<T> WithComponent<TProperty, TComponent>() where TComponent : BaseFastFormComponent<TProperty>
    {
        ComponentType = typeof(TComponent);

        return this;
    }

    public FastPropertyConfiguration<T> WithAdditionalOption(string name, object data)
    {
        if (AdditionalOptions == null)
            AdditionalOptions = new();
        
        AdditionalOptions.Set(name, data);
        
        return this;
    }

    public FastPropertyConfiguration<T> WithDefaultComponent()
    {
        var propertyType = FormHelper.GetPropertyInfo(Field).PropertyType;
        var defaultComponent = DefaultComponentSelector.GetDefault(propertyType);

        if (defaultComponent == null)
            throw new ArgumentException($"There is no default component registered for the type: '{propertyType.FullName}'");

        ComponentType = defaultComponent;
        
        return this;
    }

    public FastPropertyConfiguration<T> WithValidation<TProperty>(Func<TProperty, ValidationResult?> validator)
    {
        Validators.Add(new CustomFastValidator<TProperty>(validator));
        return this;
    }

    public FastPropertyConfiguration<T> WithValidation(IFastValidator validator)
    {
        Validators.Add(validator);
        return this;
    }

    public FastPropertyConfiguration<T> WithSection(string name, string? icon = null)
    {
        SectionConfiguration = new()
        {
            Name = name,
            Icon = icon
        };
        
        return this;
    }
    
    public FastPropertyConfiguration<T> WithPage(string name)
    {
        PageConfiguration = new()
        {
            Name = name
        };
        
        return this;
    }
}