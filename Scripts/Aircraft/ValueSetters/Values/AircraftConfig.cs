using Godot;
using System;

namespace Aircraft.ValueSetters.Values;

[GlobalClass]
public partial class AircraftConfig : AbstractValue
{
    [Export] public NodePath AircraftPath { get; set; }
    [Export] public string Property { get; set; }

    public override dynamic GetValue(ValueSetter valueSetter)
    {
        return valueSetter.GetNode<Aircraft>(AircraftPath).Config[Property];
    }

    public override void SetValue(dynamic value, ValueSetter valueSetter)
    {
        throw new Exception("Cannot set a raw value - it is read-only");
    }
}