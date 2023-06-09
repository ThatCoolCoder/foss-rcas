using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> Intersperse<T>(this IEnumerable<T> items, T separator)
    {
        var first = false;
        foreach (var item in items)
        {
            yield return item;
            if (!first) yield return separator;
        }
    }
}