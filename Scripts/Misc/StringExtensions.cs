using System;

public static class StringExtensions
{
    public static string ReplaceFirst(this string original, string find, string replace)
    {
        var index = original.IndexOf(find);
        if (index < 0) return original;

        else return original.Substring(0, index) + replace + original.Substring(index + find.Length);
    }

    public static string ReplaceLast(this string original, string find, string replace)
    {
        var index = original.LastIndexOf(find);
        if (index < 0) return original;

        else return original.Substring(0, index) + replace + original.Substring(index + find.Length);
    }
}