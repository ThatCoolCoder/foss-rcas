using Godot;
using System;

namespace Aircraft.ValueSetters.Values;

[GlobalClass]
public abstract partial class AbstractValue : Resource
{
    public abstract dynamic GetValue(ValueSetter valueSetter);
    public abstract void SetValue(dynamic value, ValueSetter valueSetter);
}