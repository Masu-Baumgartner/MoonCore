namespace MoonCore.Blazor.Tailwind.LegacyForms;

public class FormComponentRepository
{
    private static readonly Dictionary<Type, Type> Items = new();

    public static void Set<TType, TComponent>() where TComponent : BaseFormComponent<TType>
        => Items[typeof(TType)] = typeof(TComponent);

    public static Type? Get<TType>() => Get(typeof(TType));

    public static Type? Get(Type type) => Items.GetValueOrDefault(type);
}