using System.Linq.Expressions;
using MoonCore.Blazor.Helpers;

namespace MoonCore.Blazor.Tailwind.Forms;

public class FormConfiguration<TForm>
{
    public List<BaseFieldConfiguration> Fields { get; } = new();
    public List<SectionConfiguration<TForm>> Sections { get; } = new();
    public List<PageConfiguration<TForm>> Pages { get; } = new();
    
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
    
    public PageConfiguration<TForm> WithPage(string name, Action<PageConfiguration<TForm>>? onConfigure = null)
    {
        var pageSection = new PageConfiguration<TForm>()
        {
            Name = name
        };
        
        onConfigure?.Invoke(pageSection);
        
        Pages.Add(pageSection);
        return pageSection;
    }

    public CompiledFormConfiguration Compile(IServiceProvider provider, object model)
    {
        return new()
        {
            Fields = Fields
                .Select(x => x.Compile(provider, model))
                .ToArray(),
            Sections = Sections
                .Select(x => x.Compile(provider, model))
                .ToArray(),
            Pages = Pages
                .Select(x => x.Compile(provider, model))
                .ToArray()
        };
    }
}