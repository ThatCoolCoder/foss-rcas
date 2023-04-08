using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public static class NodeExtensions
{
    public static List<Node> GetChildNodeList(this Node node)
    {
        // Why is this not standard in Godot? GetChildren() returns a list of objects! Surely only a node can be a child of another node?

        var result = new List<Node>();
        foreach (var child in node.GetChildren())
        {
            result.Add((Node)child);
        }
        return result;
    }
}