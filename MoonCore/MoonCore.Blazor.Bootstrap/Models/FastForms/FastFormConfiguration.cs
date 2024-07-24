using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using MoonCore.Blazor.Helpers;

namespace MoonCore.Blazor.Bootstrap.Models.FastForms;

public class FastFormConfiguration<TModel>
{
    public readonly List<IFastFormPropertyConfiguration> PropertyConfigurations = new();
    public readonly List<FastFormPageConfiguration> CustomPages = new();

    public FastFormPropertyConfiguration<TModel, TProperty> AddProperty<TProperty>(Expression<Func<TModel, TProperty?>> func)
    {
        var config = new FastFormPropertyConfiguration<TModel, TProperty>()
        {
            PropertyFunc = func,
            PropertyInfo = FormHelper.GetPropertyInfo(func)
        };
        
        PropertyConfigurations.Add(config);

        return config;
    }
    
    public FastFormConfiguration<TModel> AddCustomPage(string name, RenderFragment page)
    {
        CustomPages.Add(new()
        {
            Name = name,
            OverrideComponent = page
        });
        
        return this;
    }
}