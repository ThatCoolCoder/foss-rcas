using Godot;
using System;

namespace Aircraft.ValueSetters.Sources;

[GlobalClass]
public partial class SingleOutput : AbstractValueSetterOutput
{
    [Export] public NodePath TargetNodePath { get; set; }
    [Export] public string Property { get; set; }

    public override void Apply(dynamic value, Node valueSetter)
    {
        valueSetter.GetNode(TargetNodePath).Set(Property, value);
    }
}