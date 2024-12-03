using System.Linq.Expressions;
using System.Reflection;

namespace MoonCore.Helpers;

public static class LinqHelper
{
    public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object?>> expression)
    {
        MemberExpression? memberExpression = default;

        if (expression.Body is MemberExpression body)
            memberExpression = body;
        else if (expression.Body is UnaryExpression unaryExpression &&
                 unaryExpression.NodeType == ExpressionType.Convert)
        {
            if (unaryExpression.Operand is MemberExpression operand)
                memberExpression = operand;
        }

        if (memberExpression == null)
            throw new ArgumentException("Unable to process expression");

        if (memberExpression.Member is PropertyInfo propertyInfo)
        {
            return propertyInfo;
        }

        throw new ArgumentException("Expression does not represent a property.");
    }
}