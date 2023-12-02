using Godot;
using System;

namespace Aircraft.ValueSetters.Values;

[GlobalClass]
public partial class ConstTrue : AbstractValue
{
    protected override dynamic InternalGetValue(ValueSetter valueSetter)
    {
        return 1;
    }

    protected override void InternalSetValue(dynamic value, ValueSetter valueSetter)
    {
        throw new Exceptions.CannotSetException("a raw value (true)");
    }
}