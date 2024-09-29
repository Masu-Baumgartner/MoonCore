namespace MoonCore.Blazor.Tailwind.Forms;

public class FieldConfiguration<TForm, TField> : BaseFieldConfiguration
{
    public void WithComponent<TComponent>(Action<TComponent>? onConfigure = null)
        where TComponent : BaseFormComponent<TField>
    {
        ComponentType = typeof(TComponent);
        ComponentConfigureAction = onConfigure;
    }
}