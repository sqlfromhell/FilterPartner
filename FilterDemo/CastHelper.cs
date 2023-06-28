// Ignore Spelling: Nullable

using System.Collections;
using System.Text.Json;

namespace FilterDemo;

internal static class CastHelper
{
    public static object Cast
        (this object obj, Type type)
        => obj is null || obj == DBNull.Value
            ? null
            : obj is JsonElement element
            ? element.Cast(type)
            : type.IsNullableType()
            ? Convert.ChangeType(obj, type.GetTypeOfGeneric())
            : Convert.ChangeType(obj, type);

    public static object Cast
        (this JsonElement element, Type type)
        => (
            element.ValueKind == JsonValueKind.String
                ? element.GetString()
                : element.GetRawText()
        ).Cast(type);

    public static IEnumerable CastTo
        (this IEnumerable lst, Type type)
    {
        if (lst is null) yield break;

        foreach (var item in lst)
            yield return item.Cast(type);
    }

    public static IEnumerable CastTo
        (this JsonElement element, Type type)
    {
        if (element.ValueKind != JsonValueKind.Array) yield break;

        foreach (var item in element
            .EnumerateArray())
            yield return item.Cast(type);
    }

    public static Type GetTypeOfGeneric
        (this Type type)
        => type.GetGenericArguments()[0];

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
        => element.CastTo(type)
            .ToArrayOf(type);
}