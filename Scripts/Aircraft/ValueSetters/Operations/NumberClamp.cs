using Godot;
using System;

namespace Aircraft.ValueSetters.Operations;

[GlobalClass]
public partial class NumberClamp : AbstractValueSetterOperation
{
    [Export] public Values.AbstractValue Number { get; set; }
    [Export] public Values.AbstractValue LowerNumber { get; set; }
    [Export] public Values.AbstractValue UpperNumber { get; set; }
    [Export] public Values.AbstractValue Output { get; set; }

    public override void Execute(ValueSetter valueSetter)
    {
        var value = Misc.TryCast<float>(Number.GetValue(valueSetter));
        var lower = Misc.TryCast<float>(LowerNumber.GetValue(valueSetter));
        var upper = Misc.TryCast<float>(UpperNumber.GetValue(valueSetter));

        Output.SetValue(Mathf.Clamp(value, lower, upper), valueSetter);
    }
}