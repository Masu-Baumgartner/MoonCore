using System.Linq.Expressions;
using MoonCore.Blazor.Helpers;

namespace MoonCore.Blazor.Tailwind.Forms;

public class SectionConfiguration<TForm>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<BaseFieldConfiguration> Fields { get; } = new();
    
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

    public CompiledSectionConfiguration Compile(IServiceProvider provider, object model)
    {
        return new()
        {
            Name = Name,
            Description = Description,
            Fields = Fields
                .Select(x => x.Compile(provider, model))
                .ToArray()
        };
    }
}