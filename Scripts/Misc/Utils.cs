using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Tomlet;

public static class Utils
{
    private static Random random = new Random();

    public static void LogError(string message, Node node = null)
    {
        if (node == null) GD.PrintErr($"Error: {message}");
        else GD.PrintErr($"Error: {message} ({node.GetPath().ToString()})");
        GD.Print(System.Environment.StackTrace);
    }

    public static void Assert(bool condition, string message, Node node = null)
    {
        if (!condition)
        {
            if (node == null) GD.PrintErr($"Error: {message}");
            else GD.PrintErr($"Error: {message} ({node.GetPath().ToString()})");
            GD.Print(System.Environment.StackTrace);
        }
    }

    public static T GetNodeWithWarnings<T>(Node node, NodePath nodePath, string itemDescriptor, bool tryParentFirst = false) where T : class
    {
        // Try getting a node and give some descriptive errors if we don't find it
        // if the path is null, checks if the parent is an applicable item
        // todo: should rename this
        // todo: not sure if it gives a good error message if the node is not found

        T result = null;
        if (nodePath == null || nodePath == "")
        {
            if (tryParentFirst && node.GetParent() is T _result) result = _result;
            else Utils.LogError($"No {itemDescriptor} path provided", node);
        }
        else if (node.GetNode(nodePath) is T _result) result = _result;
        else Utils.LogError($"{nodePath} is not a {itemDescriptor}", node);
        return result;
    }

    public static void DPrint(params object[] items)
    {
        // Like GD.Print but adds a space between items

        GD.Print(items.Intersperse(" ").ToArray());
    }

    public static T RandomItem<T>(IList<T> items)
    {
        return items[random.Next(0, items.Count())];
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

    public static float RoundTo(float number, float roundTo)
    {
        // Round number to a multiple of roundTo
        return Mathf.Round(number / roundTo) * roundTo;
    }

    public static float RoundToPlaces(float number, float decimalPlaces)
    {
        return RoundTo(number, Mathf.Pow(10, -decimalPlaces));
    }

    public static List<string> GetItemsInDirectory(string path, bool recursive = true)
    {
        if (!path.EndsWith("/")) path += "/";

        var dir = DirAccess.Open(path);
        var files = new List<string>();

        if (dir != null)
        {
            dir.ListDirBegin();

            while (true)
            {
                var entry = dir.GetNext();
                if (entry == "") break;

                var entryPath = dir.GetCurrentDir() + '/' + entry;

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

    public static bool HasExtension(string fileName, string extension, bool caseSensitive = false)
    {
        // Whether the filename ends in the extension. If the extension contains multiple dots then searches for multiple dots
        // Extension should not have a leading dot

        var sections = fileName.Split(".").ToList();
        var dotCount = extension.Count(".");
        var extensionSectionsCount = dotCount + 1;

        if (extensionSectionsCount > sections.Count) return false;

        var lastSections = sections.GetRange(sections.Count - extensionSectionsCount, extensionSectionsCount);
        var mode = caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
        return String.Join(".", lastSections).Equals(extension, mode);
    }

    public static string Pluralize(int count, string singular, string plural)
    {
        return count == 1 ? singular : plural;
    }

    public static object GetPropertyOrField(object obj, string name)
    {
        var member = obj.GetType().GetMember(name).FirstOrDefault((System.Reflection.MemberInfo)null);
        if (member is System.Reflection.FieldInfo fi) return fi.GetValue(obj);
        else if (member is System.Reflection.PropertyInfo pi) return pi.GetValue(obj, null);
        else throw new Exception("Didn't find property");
    }

    public static void SetPropertyOrField(object obj, string name, object value)
    {
        var member = obj.GetType().GetMember(name).FirstOrDefault((System.Reflection.MemberInfo)null);
        if (member is System.Reflection.FieldInfo fi) fi.SetValue(obj, value);
        else if (member is System.Reflection.PropertyInfo pi) pi.SetValue(obj, value);
        else if (member is null) throw new Exception("Member was not found");
        else throw new Exception("Cannot set this type of member");
    }

    public static object GetValueNested(object obj, string propertyName)
    {
        return GetValueNested(obj, propertyName.Split("."));
    }

    public static object GetValueNested(object obj, IEnumerable<string> propertySections)
    {
        var last = obj;
        foreach (var section in propertySections)
        {
            last = GetPropertyOrField(last, section);
        }
        return last;
    }

    public static void SetValueNested(object obj, string propertyName, object value)
    {
        SetValueNested(obj, propertyName.Split("."), value);
    }

    public static void SetValueNested(object obj, IEnumerable<string> propertySections, object value)
    {
        // Set value nested, if it finds a struct then it reassigns the struct so that the value actually saves

        var last = obj;
        var foundStruct = false;
        var withinStructAndParent = new List<object>(); // tree within the struct, plus the one level above the struct
        foreach (var section in propertySections.Take(propertySections.Count() - 1))
        {
            var next = GetPropertyOrField(last, section);
            if (!next.GetType().IsClass) foundStruct = true;
            if (foundStruct) withinStructAndParent.Add(last);
            last = next;
        }
        if (foundStruct) withinStructAndParent.Add(last);

        var member = last.GetType().GetMember(propertySections.Last()).FirstOrDefault((System.Reflection.MemberInfo)null);
        if (member is System.Reflection.FieldInfo fi) fi.SetValue(last, value);
        else if (member is System.Reflection.PropertyInfo pi) pi.SetValue(last, value);
        else if (member is null) throw new Exception("Property was not found");
        else throw new Exception("Cannot set this type of property");

        if (foundStruct)
        {
            // Lazily go reassigning things back up the tree. Not super efficient but meh
            for (int i = withinStructAndParent.Count() - 1; i >= 1; i--)
            {
                var childVal = withinStructAndParent[i];
                var parentVal = withinStructAndParent[i - 1];

                SetPropertyOrField(parentVal, propertySections.ToList()[i - 1], childVal);
            }
        }

    }
}