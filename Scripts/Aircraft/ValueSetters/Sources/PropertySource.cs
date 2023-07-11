using Godot;
using System;

namespace Aircraft.ValueSetters.Sources;

[GlobalClass]
public partial class PropertySource : AbstractValueSource
{
    [Export] public NodePath NodePath { get; set; }
    [Export] public string Property { get; set; }

    public override dynamic GetValue(Node valueSetter)
    {
        return valueSetter.GetNode(NodePath).Get(Property);
    }
}