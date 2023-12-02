using Godot;
using System;

namespace Aircraft.ValueSetters.Operations;

[GlobalClass]
public partial class If : AbstractValueSetterOperation
{
    [Export] public Values.AbstractValue Input { get; set; }
    [Export] public Values.AbstractValue TrueCase { get; set; }
    [Export] public Values.AbstractValue FalseCase { get; set; }
    [Export] public Values.AbstractValue Output { get; set; }

    public override void Execute(ValueSetter valueSetter)
    {
        var input = Misc.TryCast<float>(Input.GetValue(valueSetter));
        Output.SetValue(input > 0 ? TrueCase.GetValue(valueSetter) : FalseCase.GetValue(valueSetter), valueSetter);
    }
}