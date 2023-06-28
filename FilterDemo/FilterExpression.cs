// Ignore Spelling: Nullable

using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace FilterDemo;

public class FilterExpression
{
    public string Name { get; set; }
    public FilterOperator Operator { get; set; }
    public object Value { get; set; }

    public IQueryable<T> Apply<T>
        (IQueryable<T> query)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var lambda = Expression.Lambda<Func<T, bool>>(
            Apply<T>(parameter), parameter);
        return query.Where(lambda);
    }

    private Expression Apply<T>
        (ParameterExpression parameter)
    {
        var propertyInfo = GetProperty<T>();

        var left = Expression.Property(parameter, propertyInfo);

        var right = WhenEquals(propertyInfo)
            ?? WhenNullable<T>(propertyInfo)
            ?? WhenIEnumerable(propertyInfo)
            ?? WhenJsonElement(propertyInfo)
            ?? WhenDefault(propertyInfo);

        return ApplyOperator(left, right, propertyInfo.PropertyType);
    }

    private Expression ApplyOperator
        (MemberExpression left, Expression right, Type type)
        => Operator switch
        {
            FilterOperator.Equals
                => Expression.Equal(left, right),
            FilterOperator.NotEquals
                => Expression.NotEqual(left, right),
            FilterOperator.GreaterThan
                => Expression.GreaterThan(left, right),
            FilterOperator.LessThan
                => Expression.LessThan(left, right),
            FilterOperator.GreaterThanOrEqual
                => Expression.GreaterThanOrEqual(left, right),
            FilterOperator.LessThanOrEqual
                => Expression.LessThanOrEqual(left, right),
            FilterOperator.StartsWith
                => Expression.Call(left, typeof(string).GetMethod("StartsWith",
                    new[] { typeof(string) }), right),
            FilterOperator.EndsWith
                => Expression.Call(left, typeof(string).GetMethod("EndsWith",
                    new[] { typeof(string) }), right),
            FilterOperator.Contains
                => Expression.Call(left, typeof(string).GetMethod("Contains",
                    new[] { typeof(string) }), right),
            FilterOperator.NotContains
                => Expression.Not(Expression.Call(left, typeof(string).GetMethod("Contains",
                    new[] { typeof(string) }), right)),
            FilterOperator.In
                => Expression.Call(typeof(Enumerable), "Contains",
                    new[] { type }, right, left),
            FilterOperator.NotIn
                => Expression.Not(Expression.Call(typeof(Enumerable), "Contains",
                    new[] { type }, right, left)),
            _ => throw new NotSupportedException($"Filter operator {Operator} is not supported."),
        };

    private PropertyInfo GetProperty<T>()
        => typeof(T).GetProperty(Name,
        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
        ?? throw new ArgumentException($"Invalid property name: {Name}");

    private Expression WhenDefault
        (PropertyInfo propertyInfo)
        => Expression
            .Constant(Value.Cast(propertyInfo.PropertyType));

    private Expression WhenEquals
        (PropertyInfo propertyInfo)
        => Value.GetType() == propertyInfo.PropertyType
            ? Expression.Constant(Value)
            : null;

    private Expression WhenIEnumerable
        (PropertyInfo propertyInfo)
        => Value is IEnumerable lst
            ? Expression.Constant(lst
                .ToArrayOf(propertyInfo.PropertyType))
            : null;

    private Expression WhenJsonElement
        (PropertyInfo propertyInfo)
        => Value is JsonElement element
            ? Expression.Constant(element
                .ToArrayOf(propertyInfo.PropertyType))
            : null;

#pragma warning disable CS8632

    private Expression WhenNullable<T>
         (PropertyInfo propertyInfo)
         => typeof(T).IsValueType
             && propertyInfo.PropertyType is T?
             ? Expression.Convert(
                 Expression.Constant(
                     Value.Cast(
                         propertyInfo.PropertyType.GetGenericArguments()[0]
                         )
                     ),
                 propertyInfo.PropertyType
             )
             : null;

#pragma warning restore CS8632
}