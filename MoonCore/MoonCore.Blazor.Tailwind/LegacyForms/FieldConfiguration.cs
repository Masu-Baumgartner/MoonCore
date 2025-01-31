namespace MoonCore.Blazor.Tailwind.LegacyForms;

public class FieldConfiguration<TForm, TField> : BaseFieldConfiguration
{
    public void WithComponent<TComponent>(Action<TComponent>? onConfigure = null)
        where TComponent : BaseFormComponent<TField>
    {
        ComponentType = typeof(TComponent);
        ComponentConfigureAction = onConfigure;
    }
}