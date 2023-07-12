using Godot;
using System;
using System.Linq;

namespace Aircraft.ValueSetters.Operations;

[GlobalClass]
public partial class StringMultiComparison : AbstractValueSetterOperation
{
    [Export] public Values.AbstractValue String { get; set; }
    [Export] public Godot.Collections.Array<Values.AbstractValue> OtherStrings { get; set; }
    [Export] public Values.AbstractValue Output { get; set; }
    [Export] public ComparisonMode Mode { get; set; } = ComparisonMode.EqualToAny;

    public enum ComparisonMode
    {
        EqualToAny,
        EqualToAll,
        ContainsAny,
        ContainsAll,
        ContainedByAny,
        ContainedByAll,
    }

    public override void Execute(ValueSetter valueSetter)
    {
        var result = false;
        var value = String.GetValue(valueSetter);
        if (Mode == ComparisonMode.EqualToAny) result = OtherStrings.Any(x => value == x.GetValue(valueSetter));
        if (Mode == ComparisonMode.EqualToAll) result = OtherStrings.All(x => value == x.GetValue(valueSetter));

        if (Mode == ComparisonMode.ContainsAny) result = OtherStrings.Any(x => value.Contains(x.GetValue(valueSetter)));
        if (Mode == ComparisonMode.ContainsAll) result = OtherStrings.All(x => value.Contains(x.GetValue(valueSetter)));

        if (Mode == ComparisonMode.ContainedByAny) result = OtherStrings.Any(x => x.GetValue(valueSetter).Contains(value));
        if (Mode == ComparisonMode.ContainedByAll) result = OtherStrings.All(x => x.GetValue(valueSetter).Contains(value));

        Output.SetValue(Convert.ToInt32(result), valueSetter);
    }
}