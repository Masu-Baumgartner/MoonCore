using System.Linq.Expressions;
using MoonCore.Blazor.Helpers;

namespace MoonCore.Blazor.Tailwind.LegacyForms;

public class PageConfiguration<TForm>
{
    public string Name { get; set; }
    
    public List<BaseFieldConfiguration> Fields { get; } = new();
    public List<SectionConfiguration<TForm>> Sections { get; } = new();
    
    public FieldConfiguration<TForm, TField> WithField<TField>(Expression<Func<TForm, TField>> field, Action<FieldConfiguration<TForm, TField>>? onConfigure = null)
    {
        var fieldConfig = new FieldConfiguration<TForm, TField>()
        {
            Property = FormHelper.GetPropertyInfo(field!)
        };
        onConfigure?.Invoke(fieldConfig);
        
        Fields.Add(fieldConfig);
        return fieldConfig;
    }
    
    public SectionConfiguration<TForm> WithSection(string name, Action<SectionConfiguration<TForm>>? onConfigure = null)
    {
        var sectionConfig = new SectionConfiguration<TForm>()
        {
            Name = name
        };
        
        onConfigure?.Invoke(sectionConfig);
        
        Sections.Add(sectionConfig);
        return sectionConfig;
    }
    
    public CompiledPageConfiguration Compile(IServiceProvider provider, object model)
    {
        return new()
        {
            Name = Name,
            Sections = Sections
                .Select(x => x.Compile(provider, model))
                .ToArray(),
            Fields = Fields
                .Select(x => x.Compile(provider, model))
                .ToArray()
        };
    }
}