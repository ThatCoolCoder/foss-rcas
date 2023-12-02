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
        var input = Input.GetValue(valueSetter);
        if (input is int i) Output.SetValue(i > 0 ? TrueCase.GetValue(valueSetter) : FalseCase.GetValue(valueSetter), valueSetter);
        else if (input is bool b) Output.SetValue(b ? TrueCase.GetValue(valueSetter) : FalseCase.GetValue(valueSetter), valueSetter);
        else throw new Exception("Can only do if on int or bool");
    }
}