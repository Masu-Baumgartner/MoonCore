using MoonCore.Blazor.Forms.Fast.Components;
using MoonCore.Blazor.Models.Fast;

namespace MoonCore.Blazor.Extensions;

public static class FastPropertyConfigurationExtensions
{
    public static FastPropertyConfiguration<T> WithByteSize<T>(this FastPropertyConfiguration<T> configuration, string minimumUnit = "KB", string defaultUnit = "KB", int converter = 0)
    {
        configuration.WithComponent<int, ByteSizeComponent>();
        configuration.WithAdditionalOption("MinimumUnit", minimumUnit);
        configuration.WithAdditionalOption("DefaultUnit", defaultUnit);
        configuration.WithAdditionalOption("Converter", converter);
        
        return configuration;
    }
    
    public static FastPropertyConfiguration<T> WithSwitch<T>(this FastPropertyConfiguration<T> configuration)
    {
        configuration.WithComponent<bool, SwitchComponent>();
        
        return configuration;
    }
    
    public static FastPropertyConfiguration<T> WithDropdown<T, TProperty>(this FastPropertyConfiguration<T> configuration, Func<TProperty, string> displayFunc, Func<TProperty, string> searchFunc, IEnumerable<TProperty>[]? items = null) where TProperty : class
    {
        configuration.WithComponent<TProperty, DropdownComponent<TProperty>>();
        configuration.WithAdditionalOption("DisplayFunc", displayFunc);
        configuration.WithAdditionalOption("SearchFunc", searchFunc);

        if (items != null)
            configuration.WithAdditionalOption("Items", items);
        
        return configuration;
    }
    
    public static FastPropertyConfiguration<T> WithEnumSelect<T, TEnum>(this FastPropertyConfiguration<T> configuration) where TEnum : struct
    {
        configuration.WithComponent<TEnum, EnumSelectComponent<TEnum>>();
        
        return configuration;
    }
    
    public static FastPropertyConfiguration<T> WithRadialBooleanButton<T>(this FastPropertyConfiguration<T> configuration, string trueText, string falseText, string trueIcon = "", string falseIcon = "")
    {
        configuration.WithComponent<bool, RadialBooleanButton>();

        configuration.WithAdditionalOption("TrueText", trueText);
        configuration.WithAdditionalOption("FalseText", falseText);

        configuration.WithAdditionalOption("TrueIcon", trueIcon);
        configuration.WithAdditionalOption("FalseIcon", falseIcon);
        
        return configuration;
    }
    
    public static FastPropertyConfiguration<T> WithSelect<T, TProperty>(this FastPropertyConfiguration<T> configuration, Func<TProperty, string> displayField, IEnumerable<TProperty>? items = null) where TProperty : class
    {
        configuration.WithComponent<TProperty, SelectComponent<TProperty>>();
        configuration.WithAdditionalOption("DisplayField", displayField);

        if (items != null)
            configuration.WithAdditionalOption("Items", items);
        
        return configuration;
    }
    
    public static FastPropertyConfiguration<T> WithMultiSelect<T, TProperty>(this FastPropertyConfiguration<T> configuration, Func<TProperty, string> displayFunc, Func<TProperty, string> searchFunc, IEnumerable<TProperty>? items = null, Func<TProperty, string>? iconFunc = null) where TProperty : class
    {
        configuration.WithComponent<List<TProperty>, MultiSelectComponent<TProperty>>();
        configuration.WithAdditionalOption("DisplayFunc", displayFunc);
        configuration.WithAdditionalOption("SearchFunc", searchFunc);

        if (iconFunc != null)
            configuration.WithAdditionalOption("IconFunc", iconFunc);

        if (items != null)
            configuration.WithAdditionalOption("Items", items);
        
        return configuration;
    }
}