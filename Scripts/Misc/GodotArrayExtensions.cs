using Godot;
using System;
using System.Collections.Generic;

public static class GodotArrayExtensions
{
    public static List<T> ToList<[MustBeVariant] T>(this Godot.Collections.Array arr)
    {
        var result = new List<T>();
        foreach (var x in arr) result.Add(x.As<T>());
        return result;
    }

    // public static List<T> ToList<T>(this Godot.Collections.Array<T> arr)
    // {
    //     var result = new List<T>();
    //     foreach (var x in arr) result.Add(x);
    //     return result;
    // }
}
