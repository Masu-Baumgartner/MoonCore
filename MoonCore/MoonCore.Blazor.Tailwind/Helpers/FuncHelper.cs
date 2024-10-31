using System.Linq.Expressions;

namespace MoonCore.Blazor.Tailwind.Helpers;

public static class FuncHelper
{
    public static Func<T1, T2> FromProperty<T1, T2>(string propertyName)
    {
        // Build a Func<TItem, string> object
        var propertyInfo = typeof(T1).GetProperty("Id")!;
        var parameter = Expression.Parameter(typeof(T1), "item");
        var propertyAccess = Expression.Property(parameter, propertyInfo);
        var lambda = Expression.Lambda<Func<T1, T2>>(propertyAccess, parameter);

        return lambda.Compile();
    }
}