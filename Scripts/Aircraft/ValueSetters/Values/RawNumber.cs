using Godot;
using System;

namespace Aircraft.ValueSetters.Values;

[GlobalClass]
public partial class RawNumber : AbstractValue
{
    [Export] public float Value { get; set; }

    public override dynamic GetValue(ValueSetter valueSetter)
    {
        return Value;
    }

    public override void SetValue(dynamic value, ValueSetter valueSetter)
    {
        throw new Exception("Cannot set a raw value - it is read-only");
    }
}