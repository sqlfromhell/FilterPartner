using System.Linq.Expressions;

namespace FilterDemo;

public class SortExpression
{
    public SortDirection Direction { get; set; }
    public string Name { get; set; }

    public IQueryable<T> Apply<T>
        (IQueryable<T> query)
    {
        var parameter = Expression.Parameter(typeof(T), "x");

        var property = Expression.Property(parameter, Name);

        var lambda = Expression.Lambda<Func<T, object>>(
                Expression.Convert(property, typeof(object)),
                parameter
            );

        query = Direction == SortDirection.Ascending
            ? query.OrderBy(lambda)
            : (IQueryable<T>)query.OrderByDescending(lambda);

        return query;
    }
}