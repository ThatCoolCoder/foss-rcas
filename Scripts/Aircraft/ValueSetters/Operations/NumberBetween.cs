using Godot;
using System;

namespace Aircraft.ValueSetters.Operations;

[GlobalClass]
public partial class NumberBetween : AbstractValueSetterOperation
{
    [Export] public Values.AbstractValue Number { get; set; }
    [Export] public Values.AbstractValue LowerNumber { get; set; }
    [Export] public Values.AbstractValue UpperNumber { get; set; }
    [Export] public Values.AbstractValue Output { get; set; }
    [Export] public bool Inclusive { get; set; } = true;

    public override void Execute(ValueSetter valueSetter)
    {
        var value = Misc.TryCast<float>(Number.GetValue(valueSetter));
        var lower = Misc.TryCast<float>(LowerNumber.GetValue(valueSetter));
        var upper = Misc.TryCast<float>(UpperNumber.GetValue(valueSetter));

        int result = 0;
        if (Inclusive) result = lower <= value && value <= upper;
        else result = lower < value && value < upper;

        Output.SetValue(result, valueSetter);
    }
}