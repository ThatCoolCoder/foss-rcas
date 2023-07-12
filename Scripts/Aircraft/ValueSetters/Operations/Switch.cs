using Godot;
using System;

namespace Aircraft.ValueSetters.Operations;

[GlobalClass]
public partial class Switch : AbstractValueSetterOperation
{
    [Export] public Values.AbstractValue Input { get; set; }
    [Export] public Godot.Collections.Array<Values.AbstractValue> Cases { get; set; }
    [Export] public Godot.Collections.Array<Values.AbstractValue> Values { get; set; }
    [Export] public Values.AbstractValue DefaultCase { get; set; }
    [Export] public Values.AbstractValue Output { get; set; }

    public override void Execute(ValueSetter valueSetter)
    {
        var inputValue = Input.GetValue(valueSetter);
        dynamic outputValue = null;

        if (Cases.Count != Values.Count) throw new Exception("Cases and values must have the same count");

        for (int i = 0; i < Cases.Count; i++)
        {
            if (Cases[i].GetValue(valueSetter) == inputValue)
            {
                outputValue = Values[i].GetValue(valueSetter);
                break;
            }
        }

        outputValue = outputValue ?? DefaultCase.GetValue(valueSetter);
        Output.SetValue(outputValue, valueSetter);
    }
}