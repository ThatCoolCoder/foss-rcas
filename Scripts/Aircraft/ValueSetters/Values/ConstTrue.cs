using Godot;
using System;

namespace Aircraft.ValueSetters.Values;

[GlobalClass]
public partial class ConstTrue : AbstractValue
{
    public override dynamic GetValue(ValueSetter valueSetter)
    {
        return 1;
    }

    public override void SetValue(dynamic value, ValueSetter valueSetter)
    {
        throw new Exception("Cannot set a raw value - it is read-only");
    }
}