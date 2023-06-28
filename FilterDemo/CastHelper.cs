// Ignore Spelling: Nullable

using System.Collections;
using System.Text.Json;

namespace FilterDemo;

internal static class CastHelper
{
    public static object Cast(this object obj, Type type)
        => obj is null || obj == DBNull.Value
            ? null
            : Convert.ChangeType(obj, type);

    public static IEnumerable CastTo
        (this IEnumerable lst, Type type)
    {
        if (lst is null) yield break;

        foreach (var item in lst)
            yield return item.Cast(type);
    }

    public static bool HasInterface<T>
        (this Type type)
        => type.GetInterfaces()
            .Contains(typeof(T));

    public static bool IsEnumerable
        (this Type type)
        => type.HasInterface<IEnumerable>();

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

    public static Array ToArrayOf
        (this JsonElement element, Type type)
    {
        List<object> lst2 = new();

        if (element.ValueKind != JsonValueKind.Array)
            return System.Array
                .CreateInstance(type, 0);

        foreach (var item in element.EnumerateArray())
            lst2.Add(item.GetRawText().Cast(type));

        var arrayFrom = lst2.ToArray() as Array;

        var array = System.Array
            .CreateInstance(type, arrayFrom.Length);

        System.Array.Copy
            (arrayFrom, array, arrayFrom.Length);

        return array;
    }
}