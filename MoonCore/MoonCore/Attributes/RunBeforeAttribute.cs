namespace MoonCore.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RunBeforeAttribute : Attribute
{
    public Type RunBeforeType { get; }

    public RunBeforeAttribute(Type runBeforeType)
    {
        RunBeforeType = runBeforeType;
    }
}