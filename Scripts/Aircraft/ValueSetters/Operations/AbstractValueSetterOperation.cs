using Godot;
using System;

namespace Aircraft.ValueSetters.Operations;

[GlobalClass]
public abstract partial class AbstractValueSetterOperation : Resource
{
    public abstract void Execute(ValueSetter valueSetter);
}