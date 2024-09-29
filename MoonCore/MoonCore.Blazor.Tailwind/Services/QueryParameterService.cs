using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Components;
using MoonCore.Blazor.Tailwind.Helpers;

namespace MoonCore.Blazor.Tailwind.Services;

public class QueryParameterService
{
    private readonly NavigationManager Navigation;

    public QueryParameterService(NavigationManager navigation)
    {
        Navigation = navigation;
    }

    public bool HasKey(string id)
    {
        var modifier = new UrlModifier(Navigation.Uri);

        return modifier.Query.Get(id) != null;
    }

    public T GetValue<T>(string id)
    {
        var parts = Navigation.Uri.Split('?');

        if (parts.Length < 2)
            return Activator.CreateInstance<T>();

        var query = HttpUtility.ParseQueryString(parts[1]);
        var value = query.Get(id);
        
        if(value == null)
            return Activator.CreateInstance<T>();

        return (T)ParseValue(typeof(T), value);
    }

    private object ParseValue(Type targetType, string value)
    {
        if(targetType.IsEnum)
            return Enum.Parse(targetType, value, true);
        else if (targetType == typeof(string))
            return value;
        else if (targetType == typeof(int))
            return int.Parse(value);
        else if (targetType == typeof(bool))
            return bool.Parse(value);

        return Activator.CreateInstance(targetType, [])!;
    }

    public void SetValue(string id, object state)
    {
        Modify(collection =>
        {
            collection.Set(id, state.ToString()?.ToLower() ?? "null");
        });
    }

    public void Modify(Action<NameValueCollection> modifyParameter)
    {
        var modifier = new UrlModifier(Navigation.Uri);
        
        modifyParameter.Invoke(modifier.Query);
        
        var finalUrl = modifier.Build();
        
        if(finalUrl != Navigation.Uri)
            Navigation.NavigateTo(finalUrl);
    }
    
    public void UnsetState(string paramName)
    {
        Modify(collection =>
        {
            collection.Remove(paramName);
        });
    }
}