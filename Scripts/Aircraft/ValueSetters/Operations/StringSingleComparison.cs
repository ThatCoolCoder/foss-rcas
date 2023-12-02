using Godot;
using System;

namespace Aircraft.ValueSetters.Operations;

[GlobalClass]
public partial class StringSingleComparison : AbstractValueSetterOperation
{
    [Export] public Values.AbstractValue Value1 { get; set; }
    [Export] public Values.AbstractValue Value2 { get; set; }
    [Export] public Values.AbstractValue Output { get; set; }
    [Export] public ComparisonMode Mode { get; set; } = ComparisonMode.Equal;

    public enum ComparisonMode
    {
        Equal,
        Contains,
        ContainedBy
    }

    public override void Execute(ValueSetter valueSetter)
    {
        var value1 = Misc.TryCast<float>(Value1.GetValue(valueSetter));
        var value2 = Misc.TryCast<float>(Value2.GetValue(valueSetter));

        var result = false;
        if (Mode == ComparisonMode.Equal) result = value1 == value2;
        if (Mode == ComparisonMode.Contains) result = value1.Contains(value2);
        if (Mode == ComparisonMode.ContainedBy) result = value2.Contains(value1);

        Output.SetValue(Convert.ToInt32(result), valueSetter);
    }
}