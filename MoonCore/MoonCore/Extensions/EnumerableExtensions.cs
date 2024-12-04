using System.Linq.Expressions;

namespace MoonCore.Extensions;

public static class EnumerableExtensions
{
    public static T? FirstOrDefaultById<T>(this IEnumerable<T> collection, int id) where T : class
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        // Build the lambda expression at runtime
        var parameter = Expression.Parameter(typeof(T), "x");  // x => x.Id == id
        var property = Expression.Property(parameter, "Id");  // x.Id
        var constant = Expression.Constant(id, typeof(int));  // constant value of `id`
        var equality = Expression.Equal(property, constant);  // x.Id == id
        
        var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);  // (x => x.Id == id)
        var compiledLambda = lambda.Compile();
        
        // Use the constructed lambda expression in FirstOrDefault
        return collection.FirstOrDefault(compiledLambda);
    }
}