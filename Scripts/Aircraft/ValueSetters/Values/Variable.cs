using Godot;
using System;

namespace Aircraft.ValueSetters.Values;

[GlobalClass]
public partial class Variable : AbstractValue
{
    [Export] public string Name { get; set; }
    protected override dynamic InternalGetValue(ValueSetter valueSetter)
    {
        return valueSetter.GetVariable(Name);
    }

    protected override void InternalSetValue(dynamic value, ValueSetter valueSetter)
    {
        valueSetter.SetVariable(Name, value);
    }
}