using Godot;
using System;

namespace Aircraft.ValueSetters.Operations;

[GlobalClass]
public partial class Debug : AbstractValueSetterOperation
{
    [Export] public Values.AbstractValue Input { get; set; }
    [Export] public Values.AbstractValue Label { get; set; }

    public override void Execute(ValueSetter valueSetter)
    {
        var labelText = Label == null ? "no label" : Label.GetValue(valueSetter);
        GD.Print($"{labelText} (ValueSetter debug): {Input.GetValue(valueSetter)}");
    }
}