namespace MoonCore.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RunAfterAttribute : Attribute
{
    public Type RunAfterType { get; }

    public RunAfterAttribute(Type runAfterType)
    {
        RunAfterType = runAfterType;
    }
}