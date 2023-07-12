using Godot;
using System;

namespace Aircraft.ValueSetters.Values;

[GlobalClass]
public partial class Variable : AbstractValue
{
    [Export] public string Name { get; set; }
    public override dynamic GetValue(ValueSetter valueSetter)
    {
        return valueSetter.GetVariable(Name);
    }

    public override void SetValue(dynamic value, ValueSetter valueSetter)
    {
        valueSetter.SetVariable(Name, value);
    }
}