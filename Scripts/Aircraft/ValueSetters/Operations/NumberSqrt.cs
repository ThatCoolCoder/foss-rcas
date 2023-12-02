using Godot;
using System;

namespace Aircraft.ValueSetters.Operations;

[GlobalClass]
public partial class NumberSqrt : AbstractValueSetterOperation
{
    [Export] public Values.AbstractValue Input { get; set; }
    [Export] public Values.AbstractValue Output { get; set; }

    public override void Execute(ValueSetter valueSetter)
    {
        Output.SetValue(Mathf.Sqrt(Misc.TryCast<float>(Input.GetValue(valueSetter))), valueSetter);
    }
}