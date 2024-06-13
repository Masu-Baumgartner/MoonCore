using System.Reflection;

namespace MoonCore.Blazor.Helpers;

public class PropRefBinder<T>
{
    private object Data;
    private PropertyInfo Property;

    public PropRefBinder(object data, PropertyInfo property)
    {
        Data = data;
        Property = property;
    }

    public T Value
    {
        set => Property.SetValue(Data, value);
        get => (T)Property.GetValue(Data)!;
    }
    
    public T? ValueNullable
    {
        set => Property.SetValue(Data, value);
        get => (T?)Property.GetValue(Data);
    }
}