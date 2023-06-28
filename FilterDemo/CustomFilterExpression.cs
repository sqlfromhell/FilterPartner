using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace FilterDemo;

public class CustomFilterExpression
{
    [JsonIgnore]
    public Expression Expression { get; set; }

    public IQueryable<T> Apply<T>
        (IQueryable<T> query)
    {
        var lambda = Expression as Expression<Func<T, bool>>
            ?? throw new ArgumentException($"Invalid filter: {GetType().Name}");

        return query.Where(lambda);
    }
}