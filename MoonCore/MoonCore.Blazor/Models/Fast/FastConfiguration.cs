using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.Models.Fast;

public class FastConfiguration<T> where T : class
{
    public readonly List<FastPropertyConfiguration<T>> PropertyConfigurations = new();
    public readonly List<FastPageConfiguration> CustomPages = new();

    public FastPropertyConfiguration<T> AddProperty(Expression<Func<T, object?>>? field)
    {
        var fpc = new FastPropertyConfiguration<T>()
        {
            Field = field
        };
        
        PropertyConfigurations.Add(fpc);

        return fpc;
    }

    public FastConfiguration<T> AddCustomPage(string name, RenderFragment page)
    {
        CustomPages.Add(new()
        {
            Name = name,
            OverrideComponent = page
        });
        
        return this;
    }
}