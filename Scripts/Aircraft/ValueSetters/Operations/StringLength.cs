using Godot;
using System;

namespace Aircraft.ValueSetters.Operations;

[GlobalClass]
public partial class StringLength : AbstractValueSetterOperation
{
    [Export] public Values.AbstractValue Input { get; set; }
    [Export] public Values.AbstractValue Output { get; set; }

    public override void Execute(ValueSetter valueSetter)
    {
        Output.SetValue(Input.GetValue(valueSetter).Length, valueSetter);
    }
}