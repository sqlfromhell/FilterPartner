using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace FilterDemo;

public static class FilterPartner
{
    public static FilterResponse<T> ApplyFilters<T>
        (this IQueryable<T> query, FilterRequest filterParameters)
    {
        if (filterParameters == null)
            return new FilterResponse<T>(query.ToList());

        if (filterParameters.Filters != null
            && filterParameters.Filters.Any())
        {
            foreach (var filter in filterParameters.Filters)
            {
                query = query.ApplyFilter<T>(filter);
            }
        }

        if (filterParameters.CustomFilters != null
            && filterParameters.CustomFilters.Any())
        {
            foreach (var filter in filterParameters.CustomFilters)
            {
                query = query.ApplyFilter<T>(filter);
            }
        }

        if (filterParameters.Sort != null)
        {
            query = query.ApplySort(filterParameters.Sort);
        }

        if (filterParameters.Select != null && filterParameters.Select.Any())
        {
            query = query.ApplySelect(filterParameters.Select);
        }

        var page = filterParameters.Page ?? 1;
        var pageSize = filterParameters.PageSize ?? 10;

        var totalCount = filterParameters.Count ? query.Count() : 0;

        var result = new FilterResponse<T>(
            query.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList(),
                page,
                pageSize,
                totalCount);

        return result;
    }

    private static IQueryable<T> ApplyFilter<T>
        (this IQueryable<T> query, CustomFilterExpression filter)
    {
        var lambda = filter.Expression as Expression<Func<T, bool>>
            ?? throw new ArgumentException($"Invalid filter: {filter.GetType().Name}");
        return query.Where(lambda);
    }

    private static IQueryable<T> ApplyFilter<T>
        (this IQueryable<T> query, FilterExpression filter)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var lambda = Expression.Lambda<Func<T, bool>>(
            ParseFilterExpression<T>(filter, parameter), parameter);
        return query.Where(lambda);
    }

    private static IQueryable<T> ApplySelect<T>
        (this IQueryable<T> query, List<string> selectProperties)
    {
        var parameter = Expression.Parameter(typeof(T), "x");

        var propertyBindings = new List<MemberAssignment>();

        foreach (var propertyName in selectProperties)
        {
            var property = Expression.Property(parameter, propertyName);

            var member = typeof(T).GetProperty(propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            var bind = Expression.Bind(member, property);

            propertyBindings.Add(bind);
        }

        var newExpression = Expression.New(typeof(T));

        var memberInitExpression = Expression
            .MemberInit(newExpression, propertyBindings);

        var selector = Expression.Lambda<Func<T, T>>(memberInitExpression, parameter);
        return query.Select(selector);
    }

    private static IQueryable<T> ApplySort<T>
        (this IQueryable<T> query, SortExpression sort)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, sort.Name);
        var lambda = Expression.Lambda<Func<T, object>>(
                Expression.Convert(property, typeof(object)),
                parameter
            );

        query = sort.Direction == SortDirection.Ascending
            ? query.OrderBy(lambda)
            : (IQueryable<T>)query.OrderByDescending(lambda);

        return query;
    }

    private static Expression ParseFilterExpression<T>
        (FilterExpression filter, ParameterExpression parameter)
    {
        PropertyInfo propertyInfo = typeof(T).GetProperty(filter.Name,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
            ?? throw new ArgumentException($"Invalid property name: {filter.Name}");

        Expression left = Expression.Property(parameter, propertyInfo);

        Expression right;

        if (filter.Value.GetType() == propertyInfo.PropertyType)
        {
            right = Expression.Constant(filter.Value);
        }
        else if (filter.Value.GetType().IsEnumerable())
        {
            var value = filter.Value as IEnumerable;

            right = Expression.Constant(value
                .ToArrayOf(propertyInfo.PropertyType));
        }
        else
        {
            right = propertyInfo.PropertyType.IsNullableType()
                ? Expression.Convert(
                    Expression.Constant(
                        filter.Value.Cast(
                            propertyInfo.PropertyType.GetGenericArguments()[0]
                            )
                        ),
                    propertyInfo.PropertyType
                )
                : Expression
                    .Constant(filter.Value.Cast(propertyInfo.PropertyType));
        }

        Expression expression = filter.Operator switch
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
                    new[] { propertyInfo.PropertyType }, right, left),
            FilterOperator.NotIn
                => Expression.Not(Expression.Call(typeof(Enumerable), "Contains",
                    new[] { propertyInfo.PropertyType }, right, left)),
            _ => throw new NotSupportedException($"Filter operator {filter.Operator} is not supported."),
        };
        return expression;
    }
}