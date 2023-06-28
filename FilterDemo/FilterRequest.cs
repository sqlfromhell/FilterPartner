using System.Linq.Expressions;
using System.Reflection;

namespace FilterDemo;

public class FilterRequest
{
    public bool Count { get; set; }
    public List<CustomFilterExpression> CustomFilters { get; set; }
    public List<FilterExpression> Filters { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public List<string> Select { get; set; }
    public SortExpression Sort { get; set; }

    public FilterResponse<T> Apply<T>
        (List<T> lst)
        => Apply(lst.AsQueryable());

    public FilterResponse<T> Apply<T>
        (IQueryable<T> query)
    {
        Filters?
            .ForEach(f => query = f.Apply<T>(query));

        CustomFilters?
            .ForEach(f => query = f.Apply<T>(query));

        query = Sort?.Apply(query)
            ?? query;

        query = ApplySelect(query);

        var page = Page ?? 1;

        var pageSize = PageSize ?? 10;

        var totalCount = Count
            ? query.Count()
            : 0;

        var lst = query.Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new(
            lst,
            page,
            pageSize,
            totalCount
        );
    }

    public IQueryable<T> ApplySelect<T>
        (IQueryable<T> query)
    {
        if (Select is null || !Select.Any())
            return query;

        var parameter = Expression
            .Parameter(typeof(T), "x");

        List<MemberAssignment> propertyBindings = new();

        foreach (var propertyName in Select)
        {
            var property = Expression
                .Property(parameter, propertyName);

            var member = typeof(T)
                .GetProperty(propertyName,
                    BindingFlags.IgnoreCase
                    | BindingFlags.Public
                    | BindingFlags.Instance);

            var bind = Expression
                .Bind(member, property);

            propertyBindings.Add(bind);
        }

        var newExpression = Expression
            .New(typeof(T));

        var memberInitExpression = Expression
            .MemberInit(newExpression, propertyBindings);

        var selector = Expression.Lambda<Func<T, T>>
            (memberInitExpression, parameter);

        return query.Select(selector);
    }
}