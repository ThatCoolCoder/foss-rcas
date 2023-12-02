using Godot;
using System;

namespace Aircraft.ValueSetters.Values;

[GlobalClass]
public partial class RawNumber : AbstractValue
{
    [Export] public float Value { get; set; }

    protected override dynamic InternalGetValue(ValueSetter valueSetter)
    {
        return Value;
    }

    protected override void InternalSetValue(dynamic value, ValueSetter valueSetter)
    {
        throw new Exceptions.CannotSetException("a raw number");
    }
}