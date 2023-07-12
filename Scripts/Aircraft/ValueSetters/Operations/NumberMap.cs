using Godot;
using System;

namespace Aircraft.ValueSetters.Operations;

[GlobalClass]
public partial class NumberMap : AbstractValueSetterOperation
{
    [Export] public Values.AbstractValue Input { get; set; }
    [Export] public Values.AbstractValue OldMin { get; set; }
    [Export] public Values.AbstractValue OldMax { get; set; }
    [Export] public Values.AbstractValue NewMin { get; set; }
    [Export] public Values.AbstractValue NewMax { get; set; }
    [Export] public Values.AbstractValue Output { get; set; }
    [Export] public bool Clamp { get; set; } = false;

    public override void Execute(ValueSetter valueSetter)
    {
        var v = (Values.AbstractValue v) => v.GetValue(valueSetter);

        var newMin = v(NewMin);
        var newMax = v(NewMax);
        var newValue = Utils.MapNumber(v(Input), v(OldMin), v(OldMax), newMin, newMax);
        if (Clamp) newValue = Mathf.Clamp(newValue, newMin, newMax);

        Output.SetValue(newValue, valueSetter);
    }
}