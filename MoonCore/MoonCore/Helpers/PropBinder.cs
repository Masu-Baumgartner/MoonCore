using System.Reflection;

namespace MoonCore.Helpers;

/// <summary>
/// A dynamic value binder for a property. This class is useful for reflection and blazor value binding
/// </summary>
/// <typeparam name="T"></typeparam>
public class PropBinder<T>
{
    private PropertyInfo PropertyInfo;
    private object DataObject;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="propertyInfo">The property of the dataObject to bind on</param>
    /// <param name="dataObject">The instance of the class to bind a property on</param>
    public PropBinder(PropertyInfo propertyInfo, object dataObject)
    {
        PropertyInfo = propertyInfo;
        DataObject = dataObject;
    }

    public string StringValue
    {
        get => (string)PropertyInfo.GetValue(DataObject)!;
        set => PropertyInfo.SetValue(DataObject, value);
    }

    public int IntValue
    {
        get => (int)PropertyInfo.GetValue(DataObject)!;
        set => PropertyInfo.SetValue(DataObject, value);
    }

    public long LongValue
    {
        get => (long)PropertyInfo.GetValue(DataObject)!;
        set => PropertyInfo.SetValue(DataObject, value);
    }

    public bool BoolValue
    {
        get => (bool)PropertyInfo.GetValue(DataObject)!;
        set => PropertyInfo.SetValue(DataObject, value);
    }

    public DateTime DateTimeValue
    {
        get => (DateTime)PropertyInfo.GetValue(DataObject)!;
        set => PropertyInfo.SetValue(DataObject, value);
    }

    public T Class
    {
        get => (T)PropertyInfo.GetValue(DataObject)!;
        set => PropertyInfo.SetValue(DataObject, value);
    }

    public double DoubleValue
    {
        get => (double)PropertyInfo.GetValue(DataObject)!;
        set => PropertyInfo.SetValue(DataObject, value);
    }
}