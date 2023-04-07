using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class Utils
{
    private static Random random = new Random();

    public static void LogError(string message, Node node = null)
    {
        if (node == null) GD.PrintErr($"Error: {message}");
        else GD.PrintErr($"Error: {message} ({node.GetPath().ToString()})");
        GD.PrintStack();
    }

    public static T RandomItem<T>(List<T> items)
    {
        return items[random.Next(0, items.Count)];
    }

    public static float MapNumber(float num, float oldMin, float oldMax, float newMin, float newMax)
    {
        return newMin + (num - oldMin) * (newMax - newMin) / (oldMax - oldMin);
    }

    public static float ConvergeValue(float value, float target, float increment)
    {
        var delta = value - target;
        if (Mathf.Abs(delta) < increment) return target;
        else return value + -Mathf.Sign(value - target) * increment;
    }


    public static float MirrorNumber(float number, float mirrorValue)
    {
        if (number < mirrorValue) return number;
        else return mirrorValue - (number - mirrorValue);
    }

    public static float WrapNumber(float number, float min, float max)
    {
        float delta = max - min;
        while (number >= max) number -= delta;
        while (number < min) number += delta;
        return number;
    }

    public static float RoundNumber(float number, float roundTo)
    {
        return Mathf.Round(number / roundTo) * roundTo;
    }

    public static List<string> GetItemsInDirectory(string path, bool recursive = true)
    {
        var dir = new Directory();
        var files = new List<string>();

        if (!path.EndsWith("/")) path += "/";

        if (dir.Open(path) == Error.Ok)
        {
            dir.ListDirBegin();

            while (true)
            {
                var entry = dir.GetNext();
                if (entry == "") break;

                var entryPath = dir.GetCurrentDir() + entry;

                if (dir.CurrentIsDir())
                {
                    if (recursive && entry != "." && entry != "..") files.AddRange(GetItemsInDirectory(entryPath, recursive));
                }
                else files.Add(entryPath);
            }

            dir.ListDirEnd();
        }

        return files;
    }

    public static (string, string) SplitAtExtension(string fileName)
    {
        var sections = fileName.Split(".").ToList();
        var extension = sections.Last();
        sections.RemoveAt(sections.Count - 1);
        return (String.Join(".", sections), extension);
    }
}