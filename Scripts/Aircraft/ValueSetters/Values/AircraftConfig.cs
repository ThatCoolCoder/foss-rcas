using Godot;
using System;

namespace Aircraft.ValueSetters.Values;

[GlobalClass]
public partial class AircraftConfig : AbstractValue
{
    [Export] public NodePath AircraftPath { get; set; }
    [Export] public string Property { get; set; }

    protected override dynamic InternalGetValue(ValueSetter valueSetter)
    {
        return valueSetter.GetNode<Aircraft>(AircraftPath).Config[Property];
    }

    protected override void InternalSetValue(dynamic value, ValueSetter valueSetter)
    {
        throw new Exceptions.CannotSetException("an aircraft config value");
    }
}