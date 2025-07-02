namespace MoonCore.Yaml;

[AttributeUsage(AttributeTargets.Property)]
public class YamlCommentAttribute : Attribute
{
    public string Comment { get; }

    public YamlCommentAttribute(string comment)
    {
        Comment = comment;
    }
}