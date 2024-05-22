namespace MoonCoreUI.Attributes;

public class RadioButtonBoolAttribute : Attribute
{
    public RadioButtonBoolAttribute(string trueText, string falseText)
    {
        TrueText = trueText;
        FalseText = falseText;
    }

    public string TrueText { get; set; }
    public string FalseText { get; set; }

    public string TrueIcon { get; set; } = "";
    public string FalseIcon { get; set; } = "";
}