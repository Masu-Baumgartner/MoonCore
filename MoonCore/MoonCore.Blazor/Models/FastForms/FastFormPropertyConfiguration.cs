using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using MoonCore.Blazor.Forms.FastForms;
using MoonCore.Blazor.Models.FastForms.Validators;

namespace MoonCore.Blazor.Models.FastForms;

public class FastFormPropertyConfiguration<TModel, TProperty> : IFastFormPropertyConfiguration
{
    public List<IFastFormValidator> Validators { get; set; } = new();
    public Expression<Func<TModel, TProperty>> PropertyFunc { get; set; }
    public PropertyInfo PropertyInfo { get; set; }
    public Type ComponentType { get; set; }
    public object? OnConfigureFunc { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public FastFormPageConfiguration? PageConfiguration { get; set; }
    public FastFormSectionConfiguration? SectionConfiguration { get; set; }


    public FastFormPropertyConfiguration<TModel, TProperty> WithComponent<TComponent>(Action<TComponent>? onConfigure = null) where TComponent : FastFormBaseComponent<TProperty>
    {
        ComponentType = typeof(TComponent);
        OnConfigureFunc = onConfigure;
            
        return this;
    }

    public FastFormPropertyConfiguration<TModel, TProperty> WithDefaultComponent()
    {
        ComponentType = DefaultComponentRegistry.Get(typeof(TProperty))!;

        if (ComponentType == null)
            throw new ArgumentException($"No default component registered for type: {typeof(TProperty)}");
        
        return this;
    }

    public FastFormPropertyConfiguration<TModel, TProperty> WithName(string name)
    {
        Name = name;
        return this;
    }

    public FastFormPropertyConfiguration<TModel, TProperty> WithDescription(string description)
    {
        Description = description;
        return this;
    }

    public FastFormPropertyConfiguration<TModel, TProperty> WithValidation(IFastFormValidator validator)
    {
        Validators.Add(validator);
        return this;
    }
    
    public FastFormPropertyConfiguration<TModel, TProperty> WithValidation(Func<TProperty, ValidationResult?> validator)
    {
        return WithValidation(new CustomValidator<TProperty>()
        {
            Func = validator
        });
    }
    
    public FastFormPropertyConfiguration<TModel, TProperty> WithSection(string name, string? icon = null)
    {
        SectionConfiguration = new()
        {
            Name = name,
            Icon = icon
        };
        
        return this;
    }
    
    public FastFormPropertyConfiguration<TModel, TProperty> WithPage(string name)
    {
        PageConfiguration = new()
        {
            Name = name
        };
        
        return this;
    }
}