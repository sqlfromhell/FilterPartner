// Ignore Spelling: Nullable

using System.Collections;

namespace FilterDemo;

internal static class CastHelper
{
    public static object Cast(this object obj, Type type)
        => obj is null || obj == DBNull.Value
            ? null
            : Convert.ChangeType(obj, type);

    public static T Cast<T>(this object obj)
        => (T)obj.Cast(typeof(T));

    public static IEnumerable CastTo
        (this IEnumerable lst, Type type)
    {
        if (lst is null) yield break;

        foreach (var item in lst)
            yield return item.Cast(type);
    }

    public static IEnumerable<T> CastTo<T>
        (this IEnumerable lst)
    {
        if (lst is null) yield break;

        foreach (var item in lst)
            yield return item.Cast<T>();
    }

    public static bool HasInterface<T>
        (this Type type)
        => type.GetInterfaces()
            .Contains(typeof(T));

    public static bool HasInterface
        (this Type type, Type typeInterface)
        => type.GetInterfaces()
            .Contains(typeInterface);

    public static bool IsEnumerable
        (this Type type)
        => type.HasInterface<IEnumerable>();

    public static bool IsEnumerable<T>
        (this Type type)
        => type.HasInterface<IEnumerable<T>>();

    public static bool IsNullableType
        (this Type type)
        => type.IsGenericType
            && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));

    public static Array ToArrayOf
        (this IEnumerable lst, Type type)
    {
        List<object> lst2 = new();

        foreach (var item in lst.CastTo(type))
            lst2.Add(item);

        var arrayFrom = lst2.ToArray() as Array;

        var array = Array
            .CreateInstance(type, arrayFrom.Length);

        Array.Copy(arrayFrom, array, arrayFrom.Length);

        return array;
    }

    public static Array ToArrayOf<T>
        (this IEnumerable lst)
        => lst.ToArrayOf(typeof(T));
}