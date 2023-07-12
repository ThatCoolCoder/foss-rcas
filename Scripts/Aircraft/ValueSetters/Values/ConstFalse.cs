using Godot;
using System;

namespace Aircraft.ValueSetters.Values;

[GlobalClass]
public partial class ConstFalse : AbstractValue
{
    public override dynamic GetValue(ValueSetter valueSetter)
    {
        return 0;
    }

    public override void SetValue(dynamic value, ValueSetter valueSetter)
    {
        throw new Exception("Cannot set a raw value - it is read-only");
    }
}